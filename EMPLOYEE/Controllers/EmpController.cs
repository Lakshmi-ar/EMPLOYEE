using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;

namespace EMPLOYEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmpController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Employee> e = await SelectAllEmployees(connection);
            return Ok(e);

        }

        private async Task<IEnumerable<Employee>> SelectAllEmployees(SqlConnection connection)
        {
            return await connection.QueryAsync<Employee>("select * from Employees");
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Employee>> GetEmployees(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employee = await connection.QueryFirstAsync<Employee>("select * from Employees where employeeid=@EmployeeId", new { EmployeeId = Id});
            return Ok(employee);

        }

        [HttpPost]
        public async Task<ActionResult<List<Employee>>> CreateEmployees(Employee emp)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into Employees (firstname,lastname,age,department)values(@FirstName,@LastName,@Age,@Department)",emp);
            return Ok(await SelectAllEmployees(connection));

        }
        [HttpPut]
        public async Task<ActionResult<List<Employee>>> UpdateEmployees(Employee emp)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Update Employees set firstname=@FirstName,lastname=@LastName,age=@Age,department=@Department where employeeid=@EmployeeId" , emp);
            return Ok(await SelectAllEmployees(connection));

        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<Employee>>> DeleteEmployees(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from Employees where employeeid=@EmployeeId", new { EmployeeId = Id });
            return Ok(await SelectAllEmployees(connection));

        }
    }
}
