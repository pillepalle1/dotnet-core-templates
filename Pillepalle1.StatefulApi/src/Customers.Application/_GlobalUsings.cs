// Nuget + Cli
global using Dapper;
global using MediatR;
global using FluentValidation;
global using OneOf;

global using System.Data;
global using System.Collections.Immutable;

global using Microsoft.Data.Sqlite;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;

// Application
global using Customers.Application.Config;
global using Customers.Application.Model;
global using Customers.Application.Model.Entities;
global using Customers.Application.Services;
global using Customers.Application.Extensions;

global using Customers.Application.Services.Database;
global using Customers.Application.Services.Database.SqLite;
global using Customers.Application.Services.Database.Repositories;

global using Customers.Application.Cqrs.Common;
global using Customers.Application.Cqrs.Customers.Queries;
global using Customers.Application.Cqrs.Customers.Commands;
