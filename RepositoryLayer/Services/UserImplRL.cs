using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Interfaces;
using ModelLayer.Entity;
using RepositoryLayer.DTO;
using System.Text.Json;
using RepositoryLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using RabbitMqConsumer;
using NLog;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RepositoryLayer.Services
{
    public class UserImplRL : IUserRL
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AuthService _authService { get; set; }
        public UserContext _userContext { get; set; }
        private readonly IDatabase _redisDb;

        public UserImplRL(UserContext userContext, AuthService authService, IDatabase redisDb)
        {
            _userContext = userContext;
            _authService = authService;
            _redisDb = redisDb;
        }

        public async Task<ResponseDTO<string>> RegisterAsync(RegisterDTO _registerRequest)
        {
            logger.Info("RegisterAsync called with email: {0}", _registerRequest.email);
            PasswordHash _passwordHash = new PasswordHash();
            try
            {
                string hashedPassword = _passwordHash.passwordHashing(_registerRequest.password);
                var user = new UserEntity
                {
                    FirstName = _registerRequest.firstName,
                    LastName = _registerRequest.lastName,
                    Email = _registerRequest.email,
                    Password = hashedPassword
                };

                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();

                await _redisDb.KeyDeleteAsync("all_users");

                logger.Info("User registered successfully: {0}", _registerRequest.email);
                return new ResponseDTO<string>
                {
                    success = true,
                    message = "User registered successfully",
                    data = "User registered successfully"
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in RegisterAsync for email: {0}", _registerRequest.email);
                throw;
            }
        }

        public async Task<ResponseDTO<string>> DeleteAsync(string email)
        {
            logger.Info("DeleteAsync called for email: {0}", email);
            try
            {
                var user = _userContext.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    _userContext.Users.Remove(user);
                    await _userContext.SaveChangesAsync();

                    await _redisDb.KeyDeleteAsync("all_users");

                    logger.Info("User deleted successfully: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "User deleted successfully",
                        data = "User deleted successfully"
                    };
                }
                else
                {
                    logger.Warn("User not found for deletion: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "User not found",
                        data = "User not found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in DeleteAsync for email: {0}", email);
                throw;
            }
        }

        public async Task<ResponseDTO<string>> LoginAsync(LoginDTO loginRequest)
        {
            logger.Info("LoginAsync called for email: {0}", loginRequest.email);
            try
            {
                var user = _userContext.Users.FirstOrDefault(u => u.Email == loginRequest.email);
                if (user != null)
                {
                    PasswordHash _passwordHash = new PasswordHash();
                    bool isPasswordValid = _passwordHash.VerifyPassword(loginRequest.password, user.Password);

                    if (isPasswordValid)
                    {
                        var token = await _authService.GenerateJwtToken(user);
                        logger.Info("Login successful for email: {0}", loginRequest.email);

                        return new ResponseDTO<string>
                        {
                            success = true,
                            message = "User logged in successfully",
                            data = token
                        };
                    }
                    else
                    {
                        logger.Warn("Invalid password attempt for email: {0}", loginRequest.email);
                        return new ResponseDTO<string>
                        {
                            success = false,
                            message = "Invalid password",
                            data = "Invalid password"
                        };
                    }
                }
                else
                {
                    logger.Warn("User not found during login: {0}", loginRequest.email);
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "User not found",
                        data = "User not found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in LoginAsync for email: {0}", loginRequest.email);
                throw;
            }
        }

        public async Task<ResponseDTO<string>> GetAllUserAsync()
        {
            logger.Info("GetAllUserAsync called");
            try
            {
                string cacheKey = "all_users";
                string cachedData = await _redisDb.StringGetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    logger.Info("Users retrieved from Redis cache");
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Users retrieved successfully (cached)",
                        data = cachedData
                    };
                }

                var users = _userContext.Users.ToList();
                if (users != null)
                {
                    string serializedData = JsonSerializer.Serialize(users);
                    await _redisDb.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));

                    logger.Info("Users retrieved from DB and cached");
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Users retrieved successfully",
                        data = serializedData
                    };
                }
                else
                {
                    logger.Warn("No users found");
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "No users found",
                        data = "No users found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetAllUserAsync");
                throw;
            }
        }

        public async Task<ResponseDTO<string>> ChangePasswordAsync(string oldPass, string newPass, string Email)
        {
            logger.Info("ChangePasswordAsync called for email: {0}", Email);
            try
            {
                var user = _userContext.Users.FirstOrDefault(u => u.Email == Email);
                if (user != null)
                {
                    PasswordHash _passwordHash = new PasswordHash();
                    bool isPasswordValid = _passwordHash.VerifyPassword(oldPass, user.Password);

                    if (isPasswordValid)
                    {
                        user.Password = _passwordHash.passwordHashing(newPass);
                        _userContext.SaveChanges();

                        await _redisDb.KeyDeleteAsync("all_users");

                        logger.Info("Password changed successfully for email: {0}", Email);
                        return new ResponseDTO<string>
                        {
                            success = true,
                            message = "Password changed successfully",
                            data = "Password changed successfully"
                        };
                    }
                    else
                    {
                        logger.Warn("Invalid old password for email: {0}", Email);
                        return new ResponseDTO<string>
                        {
                            success = false,
                            message = "Invalid password",
                            data = "Invalid password"
                        };
                    }
                }
                else
                {
                    logger.Warn("User not found for ChangePasswordAsync: {0}", Email);
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "User not found",
                        data = "User not found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ChangePasswordAsync for email: {0}", Email);
                throw;
            }
        }

        public async Task<ResponseDTO<string>> ForgetPasswordAsync(string email)
        {
            logger.Info("ForgetPasswordAsync called for email: {0}", email);
            try
            {
                var user = _userContext.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    string resetToken = await _authService.GenerateJwtToken(user);
                    var emailMsg = new EmailMessage()
                    {
                        Subject = "Password Change Request",
                        Body = $"Hello From Team Fundoo, Your OTP for password change request is {resetToken}. If you haven't requested, kindly ignore.",
                        Email = email
                    };

                    var message = JsonSerializer.Serialize(emailMsg);
                    Publisher publisher = new Publisher();
                    RabbitMqConsumerService consumer = new RabbitMqConsumerService();
                    publisher.PublishToQueue("fundoo", message);

                    logger.Info("ForgetPasswordAsync succeeded for email: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Reset token generated successfully",
                        data = resetToken
                    };
                }
                else
                {
                    logger.Warn("User not found for ForgetPasswordAsync: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "User not found",
                        data = "User not found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ForgetPasswordAsync for email: {0}", email);
                throw;
            }
        }
        public async Task<ResponseDTO<string>> ResetPassword(string email, string newPassword)
        {
            
            try
            {
                logger.Info("ResetPassword called for email: {0}", email);
                var user = _userContext.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    PasswordHash _passwordHash = new PasswordHash();
                    user.Password = _passwordHash.passwordHashing(newPassword);
                    _userContext.SaveChanges();
                    await _redisDb.KeyDeleteAsync("all_users");
                    logger.Info("Password reset successfully for email: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Password reset successfully",
                        data = "Password reset successfully"
                    };
                }
                else
                {
                    logger.Warn("User not found for ResetPassword: {0}", email);
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "User not found",
                        data = "User not found"
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ResetPassword for email: {0}", email);
                throw;
            }
        }
    }
}
