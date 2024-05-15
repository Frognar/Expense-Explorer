global using System.Collections;
global using System.Diagnostics;
global using ExpenseExplorer.Application.Receipts.Commands;
global using ExpenseExplorer.Application.Tests.TestData;
global using ExpenseExplorer.Domain.Incomes;
global using ExpenseExplorer.Domain.Receipts;
global using ExpenseExplorer.Domain.Receipts.Facts;
global using ExpenseExplorer.Domain.ValueObjects;
global using ExpenseExplorer.Tests.Common.Generators.Commands;
global using FluentAssertions;
global using FsCheck.Xunit;
global using FunctionalCore.Failures;
global using FunctionalCore.Monads;
global using Version = ExpenseExplorer.Domain.ValueObjects.Version;
