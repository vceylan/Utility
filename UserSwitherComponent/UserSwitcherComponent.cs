using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Model;
using Newtonsoft.Json;

namespace UserSwitherComponent
{
    public class UserSwitcherComponent
    {
        private static string DbUsersFilePath
        {
            get
            {
                var dbFileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
                if (!Directory.Exists(dbFileDirectory))
                {
                    Directory.CreateDirectory(dbFileDirectory);
                }

                var dbFile = Path.Combine(dbFileDirectory, "users.json");
                if (File.Exists(dbFile))
                {
                    return dbFile;
                }

                var file = File.Create(dbFile);
                file.Close();

                return dbFile;
            }
        }

        private static string GlobalBilgiUserFileInfoFilePath
        {
            get
            {
                var dbFileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
                if (!Directory.Exists(dbFileDirectory))
                {
                    Directory.CreateDirectory(dbFileDirectory);
                }

                var dbFile = Path.Combine(dbFileDirectory, "fileInfo.txt");
                if (File.Exists(dbFile))
                {
                    return dbFile;
                }

                var file = File.Create(dbFile);
                file.Close();

                return dbFile;
            }
        }

        public static UserSwitcherComponent Instance { get; } = new UserSwitcherComponent();

        private UserSwitcherComponent()
        {
        }

        public List<UserSwitchModel> GetUsers()
        {
            var users = ReadJson<List<UserSwitchModel>>(DbUsersFilePath);

            if (users == null)
            {
                return new List<UserSwitchModel>();
            }

            var globalBilgiUserFilePath = File.ReadAllText(GlobalBilgiUserFileInfoFilePath);
            if (!File.Exists(globalBilgiUserFilePath))
            {
                users = users.Select(c => new UserSwitchModel { UserId = c.UserId, NtLogin = c.NtLogin, Description = c.Description, IsDefault = false }).ToList();
            }
            else
            {
                var currentDefaultUserNtLogin = File.ReadAllText(globalBilgiUserFilePath);

                foreach (var user in users)
                {
                    user.IsDefault = user.NtLogin == currentDefaultUserNtLogin;
                }
            }

            WriteJson(users, DbUsersFilePath);

            return users;
        }

        public OperationResult<string> GetDefaultUserInfoFilePath()
        {
            var result = new OperationResult<string> { Status = true };

            try
            {
                var filePath = File.ReadAllText(GlobalBilgiUserFileInfoFilePath);
                if (string.IsNullOrEmpty(filePath))
                {
                    result.Status = false;
                    result.Message = "Default user file info has not been set!";
                }
                else
                {
                    result.Data = filePath;
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Error Occurred! Exception : " + ex.Message;
                result.Data = ex.Message;
            }

            return result;
        }

        public OperationResult<string> SetDefaultUserInfoFilePath(string filePath)
        {
            var result = new OperationResult<string> { Status = true };

            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    result.Status = false;
                    result.Message = "Please enter valid file path!";
                }
                else
                {
                    var globalBilgiUserFilePath = File.ReadAllText(GlobalBilgiUserFileInfoFilePath);

                    if (!File.Exists(globalBilgiUserFilePath))
                    {
                        var file = string.IsNullOrEmpty(globalBilgiUserFilePath) ? File.Create(filePath) : File.Create(globalBilgiUserFilePath);
                        filePath = file.Name;
                        file.Close();
                    }
                    else
                    {
                        var temp = File.ReadAllText(globalBilgiUserFilePath);
                        File.Delete(globalBilgiUserFilePath);
                        var file = File.Create(filePath);
                        file.Close();
                        File.WriteAllText(filePath, temp);
                    }

                    File.WriteAllText(GlobalBilgiUserFileInfoFilePath, filePath);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Error Occurred! Exception : " + ex.Message;
                result.Data = filePath;
            }

            return result;
        }

        public OperationResult<string> SetDefaultUser(int userId)
        {
            var result = new OperationResult<string> { Status = true, Message = "User Activated!" };

            try
            {
                var globalBilgiUserFilePath = File.ReadAllText(GlobalBilgiUserFileInfoFilePath);
                var users = GetUsers();

                foreach (var user in users)
                {
                    if (user.UserId == userId)
                    {
                        user.IsDefault = true;

                        File.WriteAllText(globalBilgiUserFilePath, user.NtLogin);
                    }
                    else
                    {
                        user.IsDefault = false;
                    }
                }

                WriteJson(users, DbUsersFilePath);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Error Occurred! Exception : " + ex.Message;
                result.Data = ex.Message;
            }

            return result;
        }

        public OperationResult<string> SaveUser(UserSwitchModel selectedUser)
        {
            var result = new OperationResult<string> { Status = true, Message = "User Added!" };
            try
            {
                if (selectedUser.UserId == 0 || string.IsNullOrEmpty(selectedUser.NtLogin) || string.IsNullOrEmpty(selectedUser.Description))
                {
                    result.Status = false;
                    result.Message = "Missing user info!";
                    result.Data = "Please check UserId, NtLogin and UserFullname fields";
                }
                else
                {
                    var users = GetUsers();
                    var existUser = users.FirstOrDefault(u => u.UserId == selectedUser.UserId || u.NtLogin == selectedUser.NtLogin);

                    if (existUser == null)
                    {
                        users.Add(selectedUser);
                    }
                    else
                    {
                        var index = users.IndexOf(existUser);

                        if (existUser.UserId == selectedUser.UserId)
                        {
                            users[index].Description = selectedUser.Description;
                            users[index].NtLogin = selectedUser.NtLogin;
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Same NtLogin exist!";
                        }
                    }

                    WriteJson(users, DbUsersFilePath);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Error Occurred! Exception : " + ex.Message;
                result.Data = ex.Message;
            }

            return result;
        }

        public OperationResult<string> RemoveUser(int userId)
        {
            var result = new OperationResult<string> { Status = true, Message = "User removed!" };
            try
            {
                var users = GetUsers();
                var user = users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (user.IsDefault)
                    {
                        result.Status = false;
                        result.Message = "Default user couldn't deleted!";
                    }
                    else
                    {
                        users.Remove(user);
                        WriteJson(users, DbUsersFilePath);
                    }
                }
                else
                {
                    result.Status = false;
                    result.Message = "User couldn't found!";
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Error Occurred! Exception : " + ex.Message;
                result.Data = ex.Message;
            }

            return result;
        }

        private void WriteJson(object data, string filePath)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, json);
        }

        private T ReadJson<T>(string filePath)
        {
            var result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            return result;
        }
    }
}
