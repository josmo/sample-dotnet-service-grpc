using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.CodeAnalysis.Semantics;
using proto1.contexts;
using Proto;

namespace proto1.ServiceImpl
{
    public class EmployeeImpl : EmployeeService.EmployeeServiceBase
    {
        private EFDBContext _db;
        public EmployeeImpl(EFDBContext db)
        {
            _db = db;
        }
        public override Task<Employee> GetEmployee(EmployeeRequest request, ServerCallContext context)
        {
            var employee = _db.Employees.FirstOrDefault(emp => emp.Id == request.Id);
            return Task.FromResult(new Employee { Name = employee.Name, Age = employee.Age, Id = employee.Id, Status = employee.Status});
        }
        public override Task<DeleteEmployeeResult> DeleteEmployee(EmployeeRequest request, ServerCallContext context)
        {
            var employee = _db.Employees.FirstOrDefault(emp => emp.Id == request.Id);
            _db.Employees.Remove(employee);
            _db.SaveChanges();
            return Task.FromResult(new DeleteEmployeeResult { Success = true });
        }

        public override async Task GetEmployees(EmployeeRequest request, IServerStreamWriter<Employee> responseStream, ServerCallContext context)
        {
            var employees = _db.Employees.ToList();
            var responses = employees.Select(employee =>
                new Employee {Name = employee.Name, Age = employee.Age, Id = employee.Id, Status = employee.Status});
            
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(response);
            }
            await Task.CompletedTask;

        }

        public override async Task GetEmployeeMeetings(EmployeeRequest request, IServerStreamWriter<Meeting> responseStream, ServerCallContext context)
        {
            var meetings = _db.Meetings.Where(metting => metting.EmployeeId == request.Id);
            var responses = meetings.Select(meeting =>
                new Meeting {Id = meeting.Id, EmployeeId = meeting.EmployeeId, Text = meeting.Text, Title = meeting.Title});
            
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(response);
            }
        }

        public override Task<Employee> CreateEmployee(Employee request, ServerCallContext context)
        {
            var employee = new models.Employee { Name = request.Name, Age = request.Age, Status = request.Status}; 
            _db.Employees.Add(employee);
            _db.SaveChanges();
            request.Id = employee.Id;
            return Task.FromResult(request);
        }
    }
}