// THIS IS THE MODIFIED "REGISTER" ACTION
//    INSIDE THE "ACCOUNT" CONTROLLER:

[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    if (!Roles.RoleExists("Admin"))
                    {
                        Roles.CreateRole("Admin");
                    }
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);

//  GET THE ADMINISTRATORS LIST FROM WEB.CONFIG:
                    string sAdmins = ConfigurationManager.AppSettings["Admins"];
                    string[] oAdmins = sAdmins.Split(';');

                    WebSecurity.Login(model.UserName, model.Password);

//  ADD THE CURRENT USER TO THE "ADMIN" ROLE:
                    foreach (string sAdmin in oAdmins)
                    {
                        if (string.Compare(model.UserName, sAdmin) == 0
                                                &&
                            !Roles.IsUserInRole("Admin"))
                        {
                            Roles.AddUserToRole(model.UserName, "Admin");
                        }
                    }



                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
