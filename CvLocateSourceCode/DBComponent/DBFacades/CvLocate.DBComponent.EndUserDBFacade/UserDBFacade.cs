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
            try
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
                                return new SignResponse() { CanSignIn = false, ErrorMessage = "There is such email in this table" };
                            //create new recruiter and get new recruiter id
                            string newRecId = recManager.CreateRecruiter(command.Email, command.Password);
                            return new SignResponse() { CanSignIn = true, UserId = newRecId, UserType = UserType.Recruiter };
                        }
                    case UserType.JobSeeker:
                        break;
                }
                return null;
            }
            catch (Exception ex)
            {
               return new SignResponse() { CanSignIn = false, ErrorMessage = "Failed sign up with email: " + command.Email + ". The orginal error: " + ex.ToString() };

            }
        }

        public SignResponse SignIn(SigninCommand command)
        {

            try

            {
                if (command == null)
                    return new SignResponse() { CanSignIn = false, ErrorMessage = "Command cannot be null" };

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

                return new SignResponse() { CanSignIn = false, ErrorMessage = "Cannot find entity with such email and password" };

            }
            catch (Exception ex)
            {
                return new SignResponse() { CanSignIn = false, ErrorMessage = "Failed sign in with email: " + command.Email + ". The orginal error: " + ex.ToString() };
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
