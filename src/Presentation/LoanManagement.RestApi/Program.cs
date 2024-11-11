using loanManagement.Services.Installments;
using loanManagement.Services.Installments.Contracts.Interfaces;
using loanManagement.Services.Loans;
using loanManagement.Services.Loans.Contracts.Interfaces;
using loanManagement.Services.LoanTemplates;
using loanManagement.Services.LoanTemplates.Contracts.Interfaces;
using loanManagement.Services.Roles.Contracts.Interfaces;
using loanManagement.Services.UnitOfWorks;
using loanManagement.Services.Users;
using loanManagement.Services.Users.Contracts.Interfaces;
using LoanManagement.Application.Loans.ApplyLoanRequest;
using LoanManagement.Application.Loans.ApplyLoanRequest.Contracts;
using LoanManagement.Application.Loans.RegisterLoanRequest;
using LoanManagement.Application.Loans.RegisterLoanRequest.Contracts;
using LoanManagement.Entities.LoanTemplates;
using LoanManagement.Persistence.EF.DataContext;
using LoanManagement.Persistence.EF.Installments;
using LoanManagement.Persistence.EF.Loans;
using LoanManagement.Persistence.EF.LoanTemplates;
using LoanManagement.Persistence.EF.Roles;
using LoanManagement.Persistence.EF.Users;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EFDataContext>
(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"
    )));

builder.Services.AddScoped<InstallmentQuery , EFInstallmentQuery>();
builder.Services.AddScoped<InstallmentRepository, EFInstallmentRepository>();
builder.Services.AddScoped<InstallmentService, InstallmentAppService>();

builder.Services.AddScoped<LoanQuery , EFLoanQuery>();
builder.Services.AddScoped<LoanRepository, EFLoanRepository>();
builder.Services.AddScoped<LoanService , LoanAppService>();

builder.Services.AddScoped<LoanTemplateQuery , EFLoanTemplateQuery>();
builder.Services.AddScoped<LoanTemplateRepository, EFLoanTemplateRepository>();
builder.Services.AddScoped<LoanTemplateService, LoanTemplateAppService>();

builder.Services.AddScoped<RoleRepository, EFRoleRepository>();

builder.Services.AddScoped<UserQuery, EFUserQuery>();
builder.Services.AddScoped<UserRepository, EFUserRepository>();
builder.Services.AddScoped<UserService, UserAppService>();

builder.Services.AddScoped<ApproveLoanRequestHandler , ApproveLoanRequestCommandHandler>();
builder.Services.AddScoped<RegisterLoanRequestHandler , RegisterLoanRequestCommandHandler>();

builder.Services.AddScoped<UnitOfWork , EFUnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
