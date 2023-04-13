using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Common.Models;
using Template.Application.Features.Customers.Commands;
using Template.Application.Features.Customers.Queries;
using Template.WebApi.Controllers.V1;
using Template.WebApi.Presenters;

namespace Template.WebApi.Tests.Controllers;

public class CustomerControllerTests
{
    private readonly AutoMocker _mocker;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _mocker = new AutoMocker(MockBehavior.Strict);
        var senderMock = _mocker.GetMock<ISender>();
        _controller = new CustomerController(senderMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnCustomer_WhenCustomerExists()
    {
        // Arrange
        int customerId = 1;
        var customerResponse = new CustomerResponse(customerId,
                                                                "Test Name",
                                                                "test@example.com",
                                                                DateTime.UtcNow,
                                                                "TestUser");

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(It.Is<GetCustomerByIdQuery>(q => q.CustomerId == customerId), CancellationToken.None))
            .ReturnsAsync(customerResponse);

        // Act
        var result = await _controller.GetById(customerId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeOfType<CustomerResponse>().Subject;
        returnedCustomer.Should().BeEquivalentTo(customerResponse);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedCustomerId_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand("Test Name", "test@example.com");
        int createdCustomerId = 1;

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(command, CancellationToken.None))
            .ReturnsAsync(ResultFactory.Success(createdCustomerId));

        // Act
        var result = await _controller.Create(command, CancellationToken.None);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var resultValue = createdAtActionResult.Value as Result<int>;
        resultValue.Should().NotBeNull();
        resultValue.Data.Should().Be(createdCustomerId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenCustomerDeletedSuccessfully()
    {
        // Arrange
        int customerId = 1;

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(It.Is<DeleteCustomerCommand>(q => q.Id == customerId), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(customerId, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }


    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenCustomerUpdatedSuccessfully()
    {
        // Arrange
        var command = new UpdateCustomerCommand(1, "Updated Name", "updated@example.com");

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(command, CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(command, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetCustomersByEmail_ShouldReturnCustomers_WhenEmailIsFound()
    {
        // Arrange
        string email = "test@example.com";
        var customerSummaryResponses = new List<CustomerSummaryResponse>
        {
            new CustomerSummaryResponse(1, "Test Name 1", "test@example.com"),
            new CustomerSummaryResponse(2, "Test Name 2", "test@example.com"),
        };

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(It.Is<GetCustomersByEmailQuery>(q => q.Email == email), CancellationToken.None))
            .ReturnsAsync(customerSummaryResponses);

        // Act
        var result = await _controller.GetCustomersByEmail(email);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeOfType<List<CustomerSummaryResponse>>().Subject;
        returnedCustomers.Should().BeEquivalentTo(customerSummaryResponses);
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnPagedCustomers()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;
        var customerSummaryResponses = new List<CustomerSummaryResponse>
        {
            new CustomerSummaryResponse(1, "Test Name 1", "test@example.com"),
            new CustomerSummaryResponse(2, "Test Name 2", "test@example.com"),
        };

        var pagedCustomerResponse = new PagedCustomerResponse(
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: 1,
            TotalItems: 2,
            Items: new Collection<CustomerSummaryResponse>(customerSummaryResponses.ToList())
        );

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(It.Is<GetCustomersQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize), CancellationToken.None))
            .ReturnsAsync(pagedCustomerResponse);

        // Act
        var result = await _controller.GetAllCustomers(pageNumber, pageSize);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPagedCustomers = okResult.Value.Should().BeOfType<PagedCustomerResponse>().Subject;
        returnedPagedCustomers.Should().BeEquivalentTo(pagedCustomerResponse);
    }

    [Fact]
    public async Task Create_ShouldReturnUnprocessableEntity_WhenRequestIsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand("Test Name", "test@example.com");
        var errors = new List<string> { "Error message" };

        _mocker.GetMock<ISender>()
            .Setup(sender => sender.Send(command, CancellationToken.None))
            .ReturnsAsync(ResultFactory.Failure<int>(errors));

        // Act
        var result = await _controller.Create(command, CancellationToken.None);

        // Assert
        var unprocessableEntityResult = result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
        var problemDetails = unprocessableEntityResult.Value.Should().BeOfType<CustomProblemDetails>().Subject;

        // Get the list of error messages from CustomProblemDetails
        var problemDetailsErrors = problemDetails.Errors.SelectMany(e => e.Value).ToList();

        problemDetailsErrors.Should().BeEquivalentTo(errors);
    }
}
