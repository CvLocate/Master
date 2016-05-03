﻿using CvLocate.Common.EndUserDtoInterface.Command;
using CvLocate.Common.CommonDto;
using CvLocate.Common.EndUserDtoInterface.Response;
using CvLocate.Common.DbFacadeInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CvLocate.DBComponent.MongoDB.Managers;
using CvLocate.DBComponent.DbInterface.Managers;
using CvLocate.Common.EndUserDTO.Enums;
using CvLocate.Common.EndUserDtoInterface.Query;

namespace CvLocate.DBComponent.EndUserDBFacade
{
    public class UserDBFacade : IUserDBFacade
    {
        public SignResponse SignUp(SignUpCommand command)
        {
            if (command == null)
                return null;
            switch (command.UserType)
            {
                case UserType.Recruiter:
                    {
                        IRecruiterManager recManager = RecruiterManager.Instance;
                        //check email not exists
                        if (recManager.RecruiterEmailExists(command.Email))
                            return new SignResponse() { CanSignIn = false, Error = EndUserError.EmailAlreadyExists, ErrorMessage = "This email already exists in system" };
                        //create new recruiter and get new recruiter id
                        string newRecId = recManager.CreateRecruiter(command.Email, command.Password);
                        return new SignResponse() { CanSignIn = true, UserId = newRecId, UserType = UserType.Recruiter };
                    }
                case UserType.JobSeeker:
                    break;
            }
            return null;
        }

        public SignResponse SignIn(SigninCommand command)
        {
            if (command == null)
                return new SignResponse() { CanSignIn = false, Error = EndUserError.UnknownError, ErrorMessage = "Command cannot be null" };

            IRecruiterManager recManager = RecruiterManager.Instance;
            if (recManager.RecruiterEmailExists(command.Email))
            {
                string id = recManager.GetRecruiterByEmailAndPassword(command.Email, command.Password);
                return new SignResponse() { CanSignIn = true, UserId = id, UserType = UserType.Recruiter };
            }

            ICandidateManager canManager = CandidateManager.Instance;
            if (canManager.CandidateEmailExists(command.Email))
            {
                string id = canManager.GetCandidateByEmailAndPassword(command.Email, command.Password);
                return new SignResponse() { CanSignIn = true, UserId = id, UserType = UserType.JobSeeker };
            }

            return new SignResponse() { CanSignIn = false, Error = EndUserError.EmailPasswordNotFound, ErrorMessage = "Cannot find user with such email and password" };
        }

        public IsEmailExistInSystemResponse IsEmailExistInSystem(IsEmailExistInSystemQuery query)
        {
            try
            {
                bool emailExist = false;
                IRecruiterManager recManager = RecruiterManager.Instance;
                emailExist = recManager.RecruiterEmailExists(query.Email);
                if (!emailExist)
                {
                    ICandidateManager candidateManager = CandidateManager.Instance;
                    emailExist = candidateManager.CandidateEmailExists(query.Email);
                }
                return new IsEmailExistInSystemResponse() { EmailExistInSystem = emailExist };
            }
            catch (Exception ex)
            {
                return new IsEmailExistInSystemResponse() { Error = EndUserError.UnknownError, ErrorMessage = ex.ToString() };
            }
        }
    }
}
