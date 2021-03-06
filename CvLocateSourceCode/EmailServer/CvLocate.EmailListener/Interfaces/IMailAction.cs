﻿using CvLocate.EmailListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CvLocate.EmailListener.Interfaces
{
    public interface IMailAction
    {
        MailMessage Mail { get; set; }

        IMailActionDefinition ActionDefinition { get; }

        MailActionResult Result { get; set; }

        void DoAction();

    }
}
