using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using Moq;
using Zick.GameScheduler.Backend.Data;
using Zick.GameScheduler.Backend.Data.Models;
using Zick.GameScheduler.Backend.ResultConsumer;
using Zick.GameScheduler.Backend.ResultConsumer.Utils;
using Range = System.Range;

namespace Zick.GameScheduler.Backend.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ResultConsumerTest()
    {
        var result = JsonConverter.ReadJsonAndSerialize(File.ReadAllText("resultExample.json"));
        Assert.Pass();
    }
}