﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqConsumer.DTO
{
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Email { get; set; }
    }
}
