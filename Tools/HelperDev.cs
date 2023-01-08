using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TTS20.Models;
using System.Security.Principal;
using System.Web.Security;

namespace TTS20.Tools
{
    public class Helper
    {
        public static void SendMail(string from, List<string> toList, string subject, string messageText)
        {
            var configMail = ConfigurationManager.GetSection("MailBox") as NameValueCollection;

            if (from == "")
            {
                from = configMail["From"];
            }

            var msg = new MailMessage { From = new MailAddress(from) };

            //var to = "armyideas@gmail.com";
            var to = toList.FirstOrDefault();
            msg.To.Add(new MailAddress(to));
            msg.Subject = subject;

            foreach (var item in toList)
            {
                if (to != item.Trim())
                {
                    msg.To.Add(new MailAddress(item.Trim()));
                    //msg.CC.Add(new MailAddress(item.Trim()));
                }
            }
            msg.Body = messageText;
            msg.Priority = MailPriority.High;
            msg.IsBodyHtml = true;

            if (configMail != null)
            {
                var smtpClient = new SmtpClient(configMail["SMTP"], Int32.Parse(configMail["SMTPPort"])) { EnableSsl = false };
                try
                {
                    smtpClient.Send(msg);
                }
                catch (Exception ex)
                {
                    var cc = ex.Message;
                }
            }
        }

        public static IEnumerable<string> GetListOfMail(IEnumerable<User> users)
        {
            foreach (var item in users)
            {              
                yield return item.AccountAD.ToLower().Replace("bmst-kz\\", "") + "@borusan.com";
            }
        }

        public static string GetMailByAccountAD(string AccountAD)
        {
            
            return AccountAD.ToLower().Replace("bmst-kz\\", "") + "@borusan.com";
        }

        public static string GetNameByEmail(string email)
        {
            using (TTS20Entities db = new TTS20Entities())
            {
                return "iu"; //db.NetUsers.FirstOrDefault(f=> f.Email == email).FirstName;
            }
                
        }

        public static L_User GetAppUser(IIdentity identy)
        {
            //Test
            var TestRejim = "YES"; // YES/NO

            if (TestRejim == "YES")
            {
                using (TTS20Entities db = new TTS20Entities())
                {
                    var identyName = "tagiltsev98@gmail.com";

                    var netUser = db.NetUsers.FirstOrDefault(w => w.Email == identyName);
                    var localUser = db.L_User.Where(a => a.PrivateEmail == identyName).ToList();
                    if (netUser.EmailConfirmed != true) { return null; } // Если почта не подверждена то дальше не пускаем

                    if (localUser.Count() > 0)
                    {
                        return localUser.First();
                    }
                    else
                    {
                        string surname = netUser.Surname;
                        string firstName = netUser.FirstName;
                        string middleName = netUser.MiddleName;

                        string fio = surname + " " + firstName + " " + middleName;

                        L_User lu = new L_User()
                        {
                            Id = netUser.Id,
                            PrivateEmail = identyName,
                            NameRu = fio,
                            Admit = 0,
                            SUserId = netUser.Id,
                        };

                        db.L_User.Add(lu);
                        db.SaveChanges();

                        return lu;
                    }

                }
                   
            }

            var checkLogin = false;
            checkLogin = identy.IsAuthenticated;

            if (checkLogin)
            {
                using (TTS20Entities db = new TTS20Entities())
                {
                    var netUser = db.NetUsers.FirstOrDefault(w=> w.Email == identy.Name);
                    var localUser = db.L_User.Where(a => a.PrivateEmail == identy.Name).ToList();

                    if(netUser.EmailConfirmed != true) {return null; } // Если почта не подверждена то дальше не пускаем

                    if (localUser.Count() > 0)
                    {
                        return localUser.First();
                    }
                    else
                    {
                        string surname = netUser.Surname;
                        string firstName = netUser.FirstName;
                        string middleName = netUser.MiddleName;

                        string fio = surname + " " + firstName + " " + middleName;

                        L_User lu = new L_User()
                        {
                            Id = netUser.Id,
                            PrivateEmail = identy.Name,
                            NameRu = fio,
                            Admit = 0,
                            OldUserId = 0,
                            SUserId = netUser.Id,
                        };

                        db.L_User.Add(lu);
                        db.SaveChanges();

                        using (TTS2Entities db2 = new TTS2Entities())
                        {
                            //Временная синхронизация
                            var checkMail = db2.L_User.Where(s => s.PrivateEmail.ToLower() == lu.PrivateEmail.ToLower()).ToList();
                            if (checkMail.Count() == 0)
                            {

                                L_User l_User = new L_User()
                                {
                                    Admit = 1,
                                    PrivateEmail = lu.PrivateEmail,
                                    NameRu = fio,
                                    CompanyId = lu.CompanyId,
                                    OldUserId = 0,
                                    Id = lu.Id,
                                    SUserId = lu.Id,

                                };

                                db2.L_User.Add(l_User);
                                db2.SaveChanges();
                            }

                        }

                        return lu;
                    }
                }                        
            }
                

            return null;

        }


    }
}