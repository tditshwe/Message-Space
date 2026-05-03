using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using MessageApi.Database;
using Microsoft.AspNetCore.Authorization;
using MessageApi.Models;
using Microsoft.AspNetCore.Identity;
using MessageApi.Managers;

namespace MessageHandlingApi.Controllers
{
    [Authorize]
    [Route("messageApi/[controller]")]
    [ApiController]
    public class AccountController (IAccountManager accountManager) : ControllerBase
    {
        private readonly IAccountManager _accountManager = accountManager;
		    private readonly MessageContext Context = new MessageContext();

		    /// <summary>
		    /// Get account info
		    /// </summary>
		    // GET messageHandlingApi/Account/
		    [HttpGet]
        public IActionResult GetAccount(string username = "")
        {
            try
            {
                var usr = username == string.Empty ? User.Identity?.Name : username;

                return Ok(_accountManager.GetAccount(usr));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        // GET messageHandlingApi/Account/AccountList
        [HttpGet ("AccountList")]
        public IActionResult GetAccountList()
        {
            try
            {          
                return Ok (_accountManager.GetAccounts(User.Identity.Name));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
                
        /// <summary>
        /// Create a new account
        /// </summary>
        // POST messageHandlingApi/Account
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromBody] AccountCreate acc)
        {
            try
            {               
                return Ok(_accountManager.CreateAccount(acc));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Login to obtain user token
        /// </summary>
        // POST messageHandlingApi/Account/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Authenticate([FromBody] AccountLogin login)
        {
            try
            {
                return Ok(_accountManager.Authenticate(login));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        /// <summary>
        /// Upload profile picture
        /// </summary>
        // POST messageHandlingApi/Account/imgUpload
        [HttpPost ("imgUpload")]
        public IActionResult UploadImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Profile-pictures";
                string newPath = Path.Combine(folderName);

                // Create folder if it doesn't exist 
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)  
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        // Max size = 1 MB
                        int maxContentLength = 1024 * 1024 * 1;
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };  
                        var ext = fileName.Substring(fileName.LastIndexOf('.'));  
                        var extension = ext.ToLower();

                        if (!AllowedFileExtensions.Contains(extension))    
                            return BadRequest("Please Upload image of type .jpg,.gif,.png.");  

                        if (file.Length > maxContentLength)  
                            return BadRequest("Please Upload a file upto 1 MB.");   
                        
                        file.CopyTo(stream);

                        Account acc = Context.Account.Find(User.Identity.Name);
                        acc.ImageUrl = fullPath;
                        Context.Account.Update(acc);
                        Context.SaveChanges();
                    }  

                    return Ok ("Profile picture uploaded successfully.");   
                }

                return StatusCode(404, "Please upload an image");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Update authenticated user account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpPut]
        public IActionResult Edit([FromBody] AccountEdit acc)
        {
            try
            {
                if (acc == null)
                    return BadRequest(new ResponseBody <string>
                    {
                        Title = "Bad Request",
                        Status = 400,
                        Message = "Request body cannot be empty"
                    });

                if (string.IsNullOrWhiteSpace(acc.Name) && string.IsNullOrWhiteSpace(acc.Status))
                    return BadRequest(new ResponseBody <string>
                    {
                        Title = "Bad Request",
                        Status = 400,
                        Message = "At least one field (Name or Status) must be provided"
                    });

                var username = User.Identity.Name;
                Account edited = Context.Account.Find(username);

                if (edited == null)
                    return NotFound(new ResponseBody <string>
                    {
                        Title = "Not Found",
                        Status = 404,
                        Message = $"Account '{username}' was not found"
                    });

                if (!string.IsNullOrWhiteSpace(acc.Name))
                    edited.Name = acc.Name;

                if (!string.IsNullOrWhiteSpace(acc.Status))
                    edited.Status = acc.Status;

                Context.Account.Update(edited);
                Context.SaveChanges();

                return Ok(new ResponseBody <string>
                {
                    Title = "Updated",
                    Status = 200,
                    Message = "Account updated successfully"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Delete your account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpDelete]
        public IActionResult Delete()
        {
            try
            {
                var account = Context.Account.Find(User.Identity.Name);
                var chat = Context.Message.Where(m => m.SenderUsername == account.Username).ToList();
                var accountGroups = Context.Groups.Where(g => g.CreatorUsername == User.Identity.Name);

                // Delete all messages sent and received by account
                chat.ForEach(
                    c => Context.Message.Remove(c)
                );

                // Iterate through all groups created by the account to be deleted
                foreach (Groups gr in accountGroups)
                {
                    var accGroup = Context.AccountGroup.Where(g => g.GroupId == gr.Id).ToList();
                    
                    // If group has members
                    if (accGroup.Count() > 0)
                    {
                        // change the group creator to the first member
                        var acc = Context.Account.Find(accGroup[0].AccountUsername);
                        gr.CreatorUsername = accGroup[0].AccountUsername;
                        acc.Role = "GroupAdmin";

                        Context.Account.Update(acc);
                        Context.Groups.Update(gr);
                    }
                    else
                        Context.Groups.Remove(gr);
                }

                Context.SaveChanges();
                Context.Account.Remove(account);
                Context.SaveChanges();

                return Ok(new ResponseBody <int>
                {
                    Title = "Deleted",
                    Status = 204,
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }     
}