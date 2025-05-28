using System.Net.Http.Json;
using FluentAssertions;
using Messenger.Messages.Api.DTOs;
using Xunit;

namespace Messenger.Messages.ComponentTests;

public sealed class MessagesControllerTests(MessengerWebApplicationFactory factory)
    : IClassFixture<MessengerWebApplicationFactory>, IAsyncLifetime
{
    private readonly MessengerWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetMessages_Initially_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/messages");
        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

        // Assert
        messages.Should().BeEmpty();
    }

    [Fact]
    public async Task SendMessage_ShouldAppearInList()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var messageText = "Hello";
        var createDto = new CreateMessageRequestDto { Text = messageText };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/messages/{messageId}", createDto);
        response.EnsureSuccessStatusCode();

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>("/api/messages");

        // Assert
        messages.Should().HaveCount(1);
        messages[0].Id.Should().Be(messageId);
        messages[0].Text.Should().Be(messageText);
    }

    [Fact]
    public async Task SendTwoMessages_ShouldReturnInOrder()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var firstTextMessage = "First";
        var secondTextMessage = "Second";
        var firstMessage = new CreateMessageRequestDto { Text = firstTextMessage };
        var secondMessage = new CreateMessageRequestDto { Text = secondTextMessage };

        // Act
        await _client.PutAsJsonAsync($"/api/messages/{id1}", firstMessage);
        await _client.PutAsJsonAsync($"/api/messages/{id2}", secondMessage);

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>("/api/messages");

        // Assert
        messages.Should().HaveCount(2);
        messages[0].Id.Should().Be(id2);
        messages[0].Text.Should().Be(secondTextMessage);
        messages[1].Id.Should().Be(id1);
        messages[1].Text.Should().Be(firstTextMessage);
    }

    [Fact]
    public async Task UpsertMessage_UpdatesTextAndTime()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var initialTextMessage = "Initial";
        var updatedTextMessage = "Updated";
        var initialMessage = new CreateMessageRequestDto { Text = initialTextMessage };
        var updatedMessage = new CreateMessageRequestDto { Text = updatedTextMessage };

        // Act
        var response1 = await _client.PutAsJsonAsync($"/api/messages/{messageId}", initialMessage);
        response1.EnsureSuccessStatusCode();
        var msg1 = await response1.Content.ReadFromJsonAsync<MessageDto>();

        await Task.Delay(10);
        var response2 = await _client.PutAsJsonAsync($"/api/messages/{messageId}", updatedMessage);
        response2.EnsureSuccessStatusCode();

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>("/api/messages");

        // Assert
        msg1.Should().NotBeNull();
        messages.Should().HaveCount(1);
        msg1.Id.Should().Be(messageId);
        messages[0].Id.Should().Be(messageId);
        msg1.Text.Should().Be(initialTextMessage);
        messages[0].Text.Should().Be(updatedTextMessage);
        messages[0].UpdatedAt.Should().BeAfter(msg1.SentAt);
    }
}
