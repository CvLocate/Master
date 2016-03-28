﻿using CvLocate.Common.CoreDtoInterface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvLocate.ParsingEngine
{
    public interface ICvParser
    {
        CvParsedData ParseCv(CvFile cvFile);
    }
}
