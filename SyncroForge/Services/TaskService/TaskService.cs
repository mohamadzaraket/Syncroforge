using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Hubs;
using SyncroForge.Models;
using SyncroForge.Requests.Task;
using SyncroForge.Responses;
using Task = SyncroForge.Models.Task;

namespace SyncroForge.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<TaskHub> _hubContext;
        public TaskService(AppDbContext context, IHubContext<TaskHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<MainResponse> AddTask(AddTaskRequest request, string userPublicKey, int userId)
        {
            Department? department = await _context.Departments.Where(i => i.PublicKey == request.DepartmentIdentifier && i.IsDeleted == false).FirstOrDefaultAsync();
            if (department == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Department not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };

            }

            Employee? employee = await _context.Employees.Where(i => i.PublicKey == request.AssigneeIdentifier && i.IsDeleted == false).FirstOrDefaultAsync();
            if (employee == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Employee not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };

            }
            Status? status = await _context.Statuses.Where(i => i.PublicKey == request.StatusIdentifier && i.IsDeleted == false).FirstOrDefaultAsync();
            if (status == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "status not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };

            }
            Task? taskk = await _context.Tasks.Where(i => i.PublicKey == request.ParentTaskIdentifier && i.IsDeleted == false).FirstOrDefaultAsync();
            Task task = new Task();
            task.AssigneeId = employee.Id;
            task.CreatedById = userId;
            task.DepartmentId = department.Id;
            task.Summary = request.Summary;
            task.Description = request.Description;
            task.StatusId = status.Id;
            if (taskk != null)
            {
                task.ParentTaskId = taskk.Id;
            }
            await _context.Tasks.AddAsync(task);


            await _context.SaveChangesAsync();

            return new MainResponse()
            {
                Code = 200,
                Message = "Task added successfully",
                Status = 200,
                Success = true,
                Type = "success"
            };

        }

        public async Task<MainResponse> GetTask(string id,int userId)
        {
            Task? task = await _context.Tasks.Include(i => i.SubTasks).ThenInclude(i => i.Status).Include(i => i.SubTasks).ThenInclude(i => i.Creator).Include(i => i.SubTasks).ThenInclude(i => i.Assignee).ThenInclude(i => i.User).Include(i => i.Assignee).ThenInclude(i => i.User).Include(i => i.Creator).Include(i => i.Status).Include(i => i.ParentTask).Include(i=>i.Department).ThenInclude(i=>i.Company).Where(i => i.PublicKey == id).FirstOrDefaultAsync();
            if (task == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Task not found",
                    Status = 400,
                    Success = false,
                    Type = "not found",
                    data = task,
                };
            }
            bool isCreatedBy = false;
            Department dep = task.Department;
            if (dep.Company.CreatedBy == userId)
            {
                isCreatedBy = true;

            }
            


            return new MainResponse()
            {
                Code = 200,
                Message = "Task returned successfully",
                Status = 200,
                Success = true,
                Type = "success",
                data = new
                {
                    isCreatedBy=isCreatedBy,
                    summary = task.Summary,
                    description = task.Description,
                    status = new
                    {
                        name = task.Status.Name,
                        color = task.Status.Color,
                        backgroundColor = task.Status.BackgroundColor,
                    },
                    assignee = new
                    {
                        email = task.Assignee.User.Email,
                        profileUrl = task.Assignee.User.ProfileUrl
                    },
                    creator = new
                    {
                        email = task.Creator.Email,
                        profileUrl = task.Creator.ProfileUrl,
                        creatorId = task.Creator.Id
                    },
                    subtasks = task.SubTasks.Select(e => new
                    {
                        summary = e.Summary,
                        description = e.Description,
                        status = new
                        {
                            name = e.Status.Name,
                            color = e.Status.Color,
                            backgroundColor = e.Status.BackgroundColor,
                        },
                        assignee = new
                        {
                            email = e.Assignee.User.Email,
                            profileUrl = e.Assignee.User.ProfileUrl
                        },
                        creator = new
                        {
                            email = e.Creator.Email,
                            profileUrl = e.Creator.ProfileUrl
                        },

                    })
                },
            };
        }

        public async Task<MainResponse> GetTasks(int userId, string userPublicKey)
        {
            List<Task> task = await _context.Tasks.Include(i => i.SubTasks).Include(i => i.Status).Where(i => i.AssigneeId == userId && i.IsDeleted == false).ToListAsync();
            return new MainResponse()
            {
                Code = 200,
                Message = "Tasks returned successfully",
                Status = 200,
                Success = true,
                Type = "success",
                data = task,
            };
        }

        public async Task<MainResponse> UpdateTask(UpdateTaskRequest request, string userPublicKey, int userId)
        {
            var task = await _context.Tasks.Include(i => i.Status).Include(i => i.Assignee).ThenInclude(j => j.User)
                .FirstOrDefaultAsync(i => i.PublicKey == request.TaskIdentifier && !i.IsDeleted);

            if (task == null)
            {
                return new MainResponse
                {
                    Code = 400,
                    Message = "Task not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };
            }

            List<string> changes = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.AssigneeIdentifier))
            {
                var employee = await _context.Employees.Include(i => i.User)
                    .FirstOrDefaultAsync(i => i.PublicKey == request.AssigneeIdentifier && !i.IsDeleted);

                if (employee == null)
                {
                    return new MainResponse
                    {
                        Code = 400,
                        Message = "Employee not found",
                        Status = 400,
                        Success = false,
                        Type = "not found"
                    };
                }

                if (task.AssigneeId != employee.Id)
                {
                    changes.Add($"Assignee changed from {task.Assignee?.User.FirstName} to {employee.User.FirstName}");
                    task.AssigneeId = employee.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Summary) && task.Summary != request.Summary)
            {
                changes.Add($"Summary changed from '{task.Summary}' to '{request.Summary}'");
                task.Summary = request.Summary;
            }

            if (!string.IsNullOrWhiteSpace(request.Description) && task.Description != request.Description)
            {
                changes.Add("Description changed");
                task.Description = request.Description;
            }

            if (!string.IsNullOrWhiteSpace(request.StatusIdentifier))
            {
                var status = await _context.Statuses
                    .FirstOrDefaultAsync(i => i.PublicKey == request.StatusIdentifier && !i.IsDeleted);

                if (status == null)
                {
                    return new MainResponse
                    {
                        Code = 400,
                        Message = "Status not found",
                        Status = 400,
                        Success = false,
                        Type = "not found"
                    };
                }

                if (task.StatusId != status.Id)
                {
                    changes.Add($"Status changed from {task.Status?.Name} to {status.Name}");
                    task.StatusId = status.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ParentTaskIdentifier))
            {
                var parentTask = await _context.Tasks
                    .FirstOrDefaultAsync(i => i.PublicKey == request.ParentTaskIdentifier && !i.IsDeleted);

                if (task.ParentTaskId != parentTask?.Id)
                {
                    changes.Add($"Parent Task changed from {task.ParentTask?.Summary ?? "None"} to {parentTask?.Summary ?? "None"}");
                    task.ParentTaskId = parentTask?.Id;
                }
            }

            // Only save history if there are actual changes
            if (changes.Count > 0)
            {
               /* _context.TaskHistories.Add(new TaskHistory
                {
                    TaskId = task.Id,
                    Type = "Update",
                    EmployeeId = userId,
                    Value = string.Join("; ", changes)
                });*/
            }

            task.CreatedById = userId;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(task.PublicKey).SendAsync("TaskUpdated", new
            {
                description = task.Description,
                statusName = task.Status.Name,
            });

            return new MainResponse
            {
                Code = 200,
                Message = "Task updated successfully",
                Status = 200,
                Success = true,
                Type = "success"
            };
        }

    }
}
