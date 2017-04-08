using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using testEmployeeWebAPI.Models;

namespace testEmployeeWebAPI.Controllers
{
    /*
    WebApiConfig 類別可能需要其他變更以新增此控制器的路由，請將這些陳述式合併到 WebApiConfig 類別的 Register 方法。注意 OData URL 有區分大小寫。

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using testEmployeeWebAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<employee>("employees");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class employeesController : ODataController
    {
        private directoryEntities db = new directoryEntities();

        // GET: odata/employees
        [EnableQuery]
        public IQueryable<employee> Getemployees()
        {
            return db.employees;
        }

        // GET: odata/employees(5)
        [EnableQuery]
        public SingleResult<employee> Getemployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.employees.Where(employee => employee.id == key));
        }

        // PUT: odata/employees(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<employee> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            patch.Put(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // POST: odata/employees
        public IHttpActionResult Post(employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.employees.Add(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (employeeExists(employee.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(employee);
        }

        // PATCH: odata/employees(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<employee> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            patch.Patch(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // DELETE: odata/employees(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            employee employee = db.employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            db.employees.Remove(employee);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool employeeExists(int key)
        {
            return db.employees.Count(e => e.id == key) > 0;
        }
    }
}
