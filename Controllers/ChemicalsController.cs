using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Mail;
using FlowBaseAPI.DataLayer;
using FlowBaseAPI.Globals;
using FlowBaseAPI.Models;
using FlowBaseAPI.Dto;
using FlowBaseAPI.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowBaseAPI.Controllers
{
    [Route("chemicals")]
    public class ChemicalsController : Controller
    {

        private enum frequencyOfEmail {Daily, Weekly, Monthly}
        private readonly FlowbaseContext _context;

        public ChemicalsController(FlowbaseContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        [HttpGet(Name = "GetAllChemicals")]
        public IActionResult GetAllChemicals()
        {
            var chemicals = _context.Chemicals;
            if (chemicals == null)
            {
                return NoContent();
            }
            return new ObjectResult(chemicals);
        }

        [HttpGet("{id}", Name = "GetChemical")]
        public IActionResult GetChemical(int id)
        {
            var chemical = _context.Chemicals.FirstOrDefault(u => u.Id == id);
            if (chemical == null)
            {
                return NoContent();
            }
            return new ObjectResult(chemical);
        }

        [HttpPost("CreateChemicals",Name = "CreateChemicals")]
        public async Task<IActionResult> CreateChemicals([FromBody] List<Chemical> chemicals)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //set barcode
            foreach (var chemical in chemicals)
            {
                try {
                    chemical.Barcode = ++_context.MetaData.FirstOrDefault().MaxBarcode;
                    await _context.SaveChangesAsync();
                }
                catch(Exception e) {
                    return BadRequest($"Error: {e.InnerException}");
                }
            }

            try {
                 _context.Chemicals.AddRange(chemicals);
                await _context.SaveChangesAsync();
                //need to add each chemical one at a time to prevent bug when add multiple chemicals where barcode is duplicated
                var uniqueChemBeingAdded = chemicals.Select(x => x.ChemicalName).Distinct();
                foreach (var chemicalName in uniqueChemBeingAdded)
                {
                        var isDuplicate = _context.ChemicalFamily.Any(x => x.ChemicalName.ToLower() == chemicalName.ToLower());

                        if (!isDuplicate) {
                            if (_context.Chemicals.Any(x => x.ChemicalName.ToLower() == chemicalName.ToLower())) {
                                var currentQuantity = _context.Chemicals.Where(x => x.ChemicalName.ToLower() == chemicalName.ToLower()).Count();
                                var newChemFamily = new ChemicalFamily();
                                newChemFamily.ChemicalName = chemicalName;
                                newChemFamily.reorderThreshold = -1;
                                newChemFamily.ReorderQuantity = -1;
                                newChemFamily.Quantity = currentQuantity;
                                _context.ChemicalFamily.Add(newChemFamily);
                            }
                        } else {
                            _context.ChemicalFamily.Where(x=> x.ChemicalName.ToLower() == chemicalName.ToLower()).FirstOrDefault().Quantity = _context.ChemicalFamily.Where(x=> x.ChemicalName.ToLower() == chemicalName.ToLower()).FirstOrDefault().Quantity + chemicals.Where(x => x.ChemicalName.ToLower() == chemicalName.ToLower()).Count();
                            await _context.SaveChangesAsync();
                        }
                }
                await _context.SaveChangesAsync();
               
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/chemicals", chemicals);
        }

        [HttpPost("CreateMultipleChemicalsByQuantity", Name = "CreateMultipleChemicalsByQuantity")]
        public async Task<IActionResult> CreateMultipleChemicalsByQuantity([FromBody] ChemicalQuantityByPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createRangeOfChemicals = new List<Chemical>();
            for (var i = 0; i < payload.Quantity; i++)
            {
                try {
                    payload.NewChemical.Barcode = ++_context.MetaData.FirstOrDefault().MaxBarcode;
                    await _context.SaveChangesAsync();
                    createRangeOfChemicals.Add(payload.NewChemical);
                }
                catch(Exception e) {
                    return BadRequest($"Error: {e.InnerException}");
                }
            }

            try {
                _context.Chemicals.AddRange(createRangeOfChemicals);
                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Created("/chemicals", createRangeOfChemicals);
        }

        [HttpPut("{id}", Name = "UpdateChemical")]
        public async Task<IActionResult> UpdateChemical(int id, [FromBody] Chemical chemical)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try {
                chemical.Id = id;
                if (_context.Chemicals.Any(x => x.Id == id)) {
                    var originalChemName = _context.Chemicals.AsNoTracking().Where(x => x.Id == id)?.First()?.ChemicalName;
                    if (originalChemName != null && originalChemName.ToLower() != chemical.ChemicalName.ToLower()) {
                        if (_context.ChemicalFamily.AsNoTracking().Any(x => x.ChemicalName.ToLower() == originalChemName.ToLower()) ) {
                            _context.ChemicalFamily.Where(x => x.ChemicalName.ToLower() == originalChemName.ToLower()).First().Quantity -= 1;
                            if (_context.ChemicalFamily.Where(x => x.ChemicalName.ToLower() == originalChemName.ToLower()).First().Quantity < 0)
                                return BadRequest("Quantity below 0");
                            if (_context.ChemicalFamily.AsNoTracking().Any(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower())) {
                                _context.ChemicalFamily.Where(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower()).First().Quantity += 1;
                            } else {
                                var newChemFamily = new ChemicalFamily();
                                newChemFamily.ChemicalName = chemical.ChemicalName;
                                newChemFamily.reorderThreshold = -1;
                                newChemFamily.ReorderQuantity = -1;
                                newChemFamily.Quantity = 1;
                                _context.ChemicalFamily.Add(newChemFamily);
                            }
                        }
                    }
                }
                _context.Chemicals.Update(chemical);
                await _context.SaveChangesAsync();
            }
            catch (Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Ok(chemical);
        }

        [HttpDelete("{barcode}")]
        public async Task<IActionResult> DeleteChemical(string barcode)
        {
            long barcodeAsLong = 0;
            var isInt64 = Int64.TryParse(barcode, out barcodeAsLong);

            if (!isInt64)
                return BadRequest();

            var chemical = _context.Chemicals.Any(u => u.Barcode == barcodeAsLong) ? _context.Chemicals.FirstOrDefault(u => u.Barcode == barcodeAsLong) : null;
            if (chemical == null)
            {
                return NoContent();
            }

            try {
                _context.Chemicals.Remove(chemical);
                var disposedChem = new DisposedChemical();
                disposedChem.ChemicalName = chemical.ChemicalName;
                disposedChem.CommonName = chemical.CommonName;
                disposedChem.SiemensMaterialNumber = chemical.SiemensMaterialNumber;
                disposedChem.VendorName = chemical.VendorName;
                disposedChem.LotNumber = chemical.LotNumber;
                disposedChem.ReceiveDate = chemical.ReceiveDate;
                disposedChem.DisposalDate = DateTime.Now;
                disposedChem.ExpirationDate = chemical.ExpirationDate;
                disposedChem.ProjectCode = chemical.ProjectCode;
                disposedChem.StorageTemperature = chemical.StorageTemperature;
                disposedChem.Location = chemical.Location;
                disposedChem.Barcode = chemical.Barcode;
                _context.DisposedChemicals.Add(disposedChem);

                _context.ChemicalFamily.First(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower()).Quantity -= 1;

                if (_context.ChemicalFamily.First(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower()).Quantity < 0)
                    return BadRequest("Quantity below 0");

                await _context.SaveChangesAsync();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

            return Ok(chemical);
        }

        [HttpPost("deleteBatch")]
        public async Task<IActionResult> DeleteChemicals([FromBody] List<dtoBarcode> barcodes)
        {
            List<long> longBarcodes = new List<long>();
            foreach(var barcode in barcodes) {
                long barcodeAsLong;
                var isInt64 = Int64.TryParse(barcode.Barcode, out barcodeAsLong);
                if (!isInt64)
                    return BadRequest();
                else
                    longBarcodes.Add(barcodeAsLong);
            }

            var deletedChemicals = new List<Chemical>();
            foreach (var barcode in longBarcodes) {
                var chemical = _context.Chemicals.Any(u => u.Barcode == barcode) ? _context.Chemicals.FirstOrDefault(u => u.Barcode == barcode) : null;
                if (chemical == null)
                {
                    return NoContent();
                }

                try {
                    _context.Chemicals.Remove(chemical);
                    var disposedChem = new DisposedChemical();
                    disposedChem.ChemicalName = chemical.ChemicalName;
                    disposedChem.CommonName = chemical.CommonName;
                    disposedChem.SiemensMaterialNumber = chemical.SiemensMaterialNumber;
                    disposedChem.VendorName = chemical.VendorName;
                    disposedChem.LotNumber = chemical.LotNumber;
                    disposedChem.ReceiveDate = chemical.ReceiveDate;
                    disposedChem.DisposalDate = DateTime.Now;
                    disposedChem.ExpirationDate = chemical.ExpirationDate;
                    disposedChem.ProjectCode = chemical.ProjectCode;
                    disposedChem.StorageTemperature = chemical.StorageTemperature;
                    disposedChem.Location = chemical.Location;
                    disposedChem.Barcode = chemical.Barcode;
                    _context.DisposedChemicals.Add(disposedChem);

                    _context.ChemicalFamily.First(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower()).Quantity -= 1;

                    if (_context.ChemicalFamily.First(x => x.ChemicalName.ToLower() == chemical.ChemicalName.ToLower()).Quantity < 0)
                        return BadRequest("Quantity below 0");

                    await _context.SaveChangesAsync();
                    deletedChemicals.Add(chemical);
                }
                catch(Exception e) {
                    return BadRequest($"Error: {e.InnerException}");
                }
                
            }

            return Ok(deletedChemicals);
        }


        [HttpPost("family")]
        public IActionResult GetChemicalFamily([FromBody] dtoChemFamily name) {

            try {
                if (_context.ChemicalFamily.Any()) {
                    var chemFamily = _context.ChemicalFamily.Where(x => x.ChemicalName.ToLower() == name.ChemicalName.ToLower());
                    return new ObjectResult(chemFamily);
                }
                else {
                    return BadRequest();
                }
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }

        }

        [HttpPost("family/update")]
        public async Task<IActionResult> UpdateChemicalFamily([FromBody] dtoChemFamily updateInfo) {
            try {
                if (_context.ChemicalFamily.Any()) {
                    if (_context.ChemicalFamily.Any(x => x.ChemicalName.ToLower() == updateInfo.ChemicalName.ToLower())) {
                        var chemFamSource = _context.ChemicalFamily.Where(x => x.ChemicalName.ToLower() == updateInfo.ChemicalName.ToLower()).FirstOrDefault();
                        chemFamSource.reorderThreshold = updateInfo.reorderThreshold;
                        chemFamSource.ReorderQuantity = updateInfo.ReorderQuantity;

                        _context.ChemicalFamily.Update(chemFamSource);
                        await _context.SaveChangesAsync();

                        return Ok(chemFamSource);
                    }
                }
                return BadRequest();
            }
            catch(Exception e) {
                return BadRequest($"Error: {e.InnerException}");
            }
        }

        [HttpPost("notifications/sendExpireEmail")]
        public IActionResult SendExpirationStatusEmail() {
            
            if (!_context.ChemicalFamily.Any() || !_context.Users.Any())
                return BadRequest();
            
            var todayDateShort = DateTime.Now.ToShortDateString();
            var todayDate = DateTime.Parse(todayDateShort);
            var dayOfWeek = (int) todayDate.DayOfWeek;
            var allChemicals = _context.Chemicals.ToList();
            var chemicalsExpire = allChemicals
                                    .Where(x => (x.ExpirationDate - todayDate).TotalDays <= 1 || DateTime.Compare(x.ExpirationDate, todayDate) <= 0)
                                    .OrderByDescending(x => x.ExpirationDate)
                                    .ToList();

            //change 30 as needed, maybe by user setting
            var chemicalsSoon = allChemicals
                                    .Where(x => (x.ExpirationDate - todayDate).TotalDays >= 2)
                                    .Where(x => (x.ExpirationDate - todayDate).TotalDays < 30)
                                    .OrderBy(x => x.ExpirationDate)
                                    .ToList();

            var users = _context.Users.Where(x => x.Notifications == true).ToList();
            var emailToList = new List<User>();

            if (chemicalsExpire.Count == 0 && chemicalsSoon.Count == 0)
                return NoContent();

            foreach (var user in users) {
                if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Daily)) {
                    emailToList.Add(user);
                } else if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Weekly)) {
                    if (dayOfWeek == 1) 
                        emailToList.Add(user);
                } else if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Monthly)) {
                    if ((dayOfWeek == 1 && todayDate.Day <= 7) || (todayDate.Day == 1 && dayOfWeek <= 5))
                        emailToList.Add(user); 
                }
            }

            if (emailToList.Count == 0) {
                return NoContent();
            }

            var sbExpired = new StringBuilder();
            if (chemicalsExpire.Count > 0) {
                sbExpired.AppendLine("The following are the chemical(s) that are expired or will expire in the next day:<br /><ul>");
                foreach (var chemical in chemicalsExpire) {
                    var lineText = "<li> " + chemical.ChemicalName + " with Barcode " + chemical.Barcode.ToString() + " expires on " + chemical.ExpirationDate.ToShortDateString();
                    if (chemical.Location != null && !string.IsNullOrEmpty(chemical.Location)) {
                        lineText += " found in " + chemical.Location;
                    }
                    lineText += "</li><br />";
                    sbExpired.AppendLine(lineText);
                }
                sbExpired.Append("</ul><br />");
            }
            
            var sbSoon = new StringBuilder();
            if (chemicalsSoon.Count > 0) {
                sbSoon.AppendLine("The following are the chemical(s) that will expire in the next 2-30 days:<br /><ul>");
                foreach (var chemical in chemicalsSoon) {
                    var lineText = "<li>" + chemical.ChemicalName + " (Barcode: " + chemical.Barcode.ToString() + ") expires on " + chemical.ExpirationDate.ToShortDateString();
                    if (chemical.Location != null && !string.IsNullOrEmpty(chemical.Location)) {
                        lineText += " found in " + chemical.Location;
                    }
                    lineText += "</li><br />";
                    sbSoon.AppendLine(lineText);
                }
                sbSoon.Append("</ul><br />");
            }
            
            var emailMessageBody = "This is an automated message regarding chemical expiration status. <br /><br />";

            emailMessageBody += chemicalsExpire.Count > 0 ? sbExpired.ToString() : "";
            emailMessageBody += chemicalsSoon.Count > 0 ? sbSoon.ToString() : "";


            SmtpClient client = new SmtpClient();
            client.Host = "smtp.googlemail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            var emailer = new NetworkCredentials();
            client.Credentials = new NetworkCredential(emailer.NetworkUserEmail, emailer.NetworkUserPassword);
            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(emailer.NetworkUserEmail);
            foreach (var to in emailToList)
            {
                msg.To.Add(new MailAddress(to.Email));
            }
            msg.Subject = "FlowBase Notification: Expiration Status";
            msg.Body = emailMessageBody;
            client.Send(msg);

            return Ok(emailToList);
        }

        [HttpPost("notifications/sendReorderEmail")]
        public IActionResult SendReorderEmail() {
            if (!_context.ChemicalFamily.Any() || !_context.Users.Any())
                    return BadRequest();

            var todayDateShort = DateTime.Now.ToShortDateString();
            var todayDate = DateTime.Parse(todayDateShort);
            var dayOfWeek = (int) todayDate.DayOfWeek;
            var allChemicals = _context.Chemicals.ToList();

            var users = _context.Users.Where(x => x.Notifications == true).ToList();

            if (users.Count == 0) {
                return NoContent();
            }

            var emailToList = new List<User>();

            foreach (var user in users) {
                if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Daily)) {
                    emailToList.Add(user);
                } else if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Weekly)) {
                    if (dayOfWeek == 1) 
                        emailToList.Add(user);
                } else if (user.Frequency == Enum.GetName(typeof(frequencyOfEmail), frequencyOfEmail.Monthly)) {
                    if ((dayOfWeek == 1 && todayDate.Day <= 7) || (todayDate.Day == 1 && dayOfWeek <= 5))
                        emailToList.Add(user); 
                }
            }

            if (emailToList.Count == 0) {
                return NoContent();
            }

            var reorderChemicals = _context.ChemicalFamily.Where(x => x.Quantity <= x.reorderThreshold && x.reorderThreshold != -1).OrderBy(x => x.reorderThreshold).ToList();

            if (reorderChemicals.Count == 0)
                return NoContent();

            var sbReorder = new StringBuilder();
            if (reorderChemicals.Count > 0) {
                sbReorder.AppendLine("The following are the chemical(s) that need to be reordered:<br /><ul>");
                foreach (var chemical in reorderChemicals) {
                    var lineText = "<li> " + chemical.ChemicalName + " (Current Quantity: " + chemical.Quantity.ToString() + ") needs an order of " + chemical.ReorderQuantity;
                    lineText += "</li><br />";
                    sbReorder.AppendLine(lineText);
                }
                sbReorder.Append("</ul><br />");
            }

            var emailMessageBody = "This is an automated message regarding chemical reorder status. <br /><br />";

            emailMessageBody += reorderChemicals.Count > 0 ? sbReorder.ToString() : "";

            SmtpClient client = new SmtpClient();
            client.Host = "smtp.googlemail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            var emailer = new NetworkCredentials();
            client.Credentials = new NetworkCredential(emailer.NetworkUserEmail, emailer.NetworkUserPassword);
            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(emailer.NetworkUserEmail);
            foreach (var to in emailToList)
            {
                msg.To.Add(new MailAddress(to.Email));
            }
            msg.Subject = "FlowBase Notification: Reorder Status";
            msg.Body = emailMessageBody;
            client.Send(msg);

            return Ok(emailToList);
        }
        
        
    }

    
}
