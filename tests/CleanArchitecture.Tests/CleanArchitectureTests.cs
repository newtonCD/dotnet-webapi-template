using FluentAssertions;
using NetArchTest.Rules;

namespace CleanArchitecture.Tests;

public class CleanArchitectureTests
{
    private const string DomainNamespace = "Template.Domain";
    private const string ApplicationNamespace = "Template.Application";
    private const string InfrastructureNamespace = "Template.Infrastructure";
    private const string WebNamespace = "Template.WebApi";


    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Other_Projects()
    {
        // Arrange
        var assembly = typeof(Template.Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            WebNamespace,
        };

        //Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Other_Projects()
    {
        // Arrange
        var assembly = typeof(Template.Application.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            WebNamespace,
        };

        //Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_Have_Dependency_On_Domain()
    {
        // Arrange
        var assembly = typeof(Template.Application.AssemblyReference).Assembly;

        //Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Other_Projects()
    {
        // Arrange
        var assembly = typeof(Template.Infrastructure.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            WebNamespace,
        };

        //Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_Should_Have_Dependency_On_MediatR()
    {
        // Arrange
        var assembly = typeof(Template.WebApi.AssemblyReference).Assembly;

        //Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
}