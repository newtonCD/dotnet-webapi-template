using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Common.Models;

namespace Template.Application.Tests.Models;

public class ResultTests
{
    [Fact]
    public void PublicConstructor_Initializes_DefaultValues()
    {
        // Arrange & Act
        Result<int> result = new Result<int>();

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Errors);
        Assert.Empty(result.Errors);
        Assert.Equal(default(int), result.Data);
    }
}
