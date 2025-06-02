using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Messenger.Messages.Api.DTOs;
using Xunit;

namespace Messenger.Messages.ComponentTests;

public sealed class MessagesControllerTests(MessengerWebApplicationFactory factory)
    : IClassFixture<MessengerWebApplicationFactory>, IAsyncLifetime
{
    private const string MessagesEndpoint = "/api/messages";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly Fixture _fixture = new ();

    [Fact]
    public async Task GetMessages_Initially_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync(MessagesEndpoint);
        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

        // Assert
        messages.Should().BeEmpty();
    }

    [Fact]
    public async Task SendMessage_ShouldAppearInList()
    {
        // Arrange
        var messageId = _fixture.Create<Guid>();
        var createDto = _fixture.Create<CreateMessageRequestDto>();
        var messageText = createDto.Text;

        // Act
        var response = await _client.PutAsJsonAsync($"{MessagesEndpoint}/{messageId}", createDto);
        response.EnsureSuccessStatusCode();

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>(MessagesEndpoint);

        // Assert
        messages.Should().HaveCount(1);
        messages[0].Id.Should().Be(messageId);
        messages[0].Text.Should().Be(messageText);
    }

    [Fact]
    public async Task SendTwoMessages_ShouldReturnInOrder()
    {
        // Arrange
        var id1 = _fixture.Create<Guid>();
        var id2 = _fixture.Create<Guid>();
        var firstMessage = _fixture.Create<CreateMessageRequestDto>();
        var secondMessage = _fixture.Create<CreateMessageRequestDto>();
        var firstTextMessage = firstMessage.Text;
        var secondTextMessage = secondMessage.Text;

        // Act
        await _client.PutAsJsonAsync($"{MessagesEndpoint}/{id1}", firstMessage);
        await _client.PutAsJsonAsync($"{MessagesEndpoint}/{id2}", secondMessage);

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>(MessagesEndpoint);

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
        var messageId = _fixture.Create<Guid>();
        var initialMessage = _fixture.Create<CreateMessageRequestDto>();
        var updatedMessage = _fixture.Create<CreateMessageRequestDto>();
        var initialTextMessage = initialMessage.Text;
        var updatedTextMessage = updatedMessage.Text;
        var initialTime = _fixture.Create<DateTimeOffset>();
        var updatedTime = initialTime.AddMinutes(1);

        // Act
        var request1 = new HttpRequestMessage(HttpMethod.Put, $"{MessagesEndpoint}/{messageId}")
        {
            Content = JsonContent.Create(initialMessage),
            Headers =
            {
                { "X-Current-Time", initialTime.ToString("O") },
            },
        };
        var response1 = await _client.SendAsync(request1);
        response1.EnsureSuccessStatusCode();
        var msg1 = await response1.Content.ReadFromJsonAsync<MessageDto>();

        var request2 = new HttpRequestMessage(HttpMethod.Put, $"{MessagesEndpoint}/{messageId}")
        {
            Content = JsonContent.Create(updatedMessage),
            Headers =
            {
                { "X-Current-Time", updatedTime.ToString("O") },
            },
        };
        var response2 = await _client.SendAsync(request2);
        response2.EnsureSuccessStatusCode();

        var messages = await _client.GetFromJsonAsync<List<MessageDto>>(MessagesEndpoint);

        // Assert
        msg1.Should().NotBeNull();
        messages.Should().HaveCount(1);
        msg1.Id.Should().Be(messageId);
        messages[0].Id.Should().Be(messageId);
        msg1.Text.Should().Be(initialTextMessage);
        messages[0].Text.Should().Be(updatedTextMessage);
        messages[0].UpdatedAt.Should().BeAfter(msg1.SentAt);
    }

    [Fact]
    public async Task SendMessageWithSameText_ShouldNotUpdateTimestamp()
    {
        // Arrange
        var messageId = _fixture.Create<Guid>();
        var initialMessage = _fixture.Create<CreateMessageRequestDto>();
        var initialTime = _fixture.Create<DateTimeOffset>();
        var updatedTime = initialTime.AddMinutes(1);

        // Act
        var request1 = new HttpRequestMessage(HttpMethod.Put, $"{MessagesEndpoint}/{messageId}")
        {
            Content = JsonContent.Create(initialMessage),
            Headers =
            {
                { "X-Current-Time", initialTime.ToString("O") },
            },
        };
        var response1 = await _client.SendAsync(request1);
        response1.EnsureSuccessStatusCode();
        var msg1 = await response1.Content.ReadFromJsonAsync<MessageDto>();

        var request2 = new HttpRequestMessage(HttpMethod.Put, $"{MessagesEndpoint}/{messageId}")
        {
            Content = JsonContent.Create(initialMessage),
            Headers =
            {
                { "X-Current-Time", updatedTime.ToString("O") },
            },
        };
        var response2 = await _client.SendAsync(request2);
        response2.EnsureSuccessStatusCode();
        var msg2 = await response2.Content.ReadFromJsonAsync<MessageDto>();

        // Assert
        msg1.Should().NotBeNull();
        msg2.Should().NotBeNull();
        msg2.Text.Should().Be(msg1.Text);
        msg2.UpdatedAt.Should().Be(msg1.UpdatedAt);
        msg2.SentAt.Should().Be(msg1.SentAt);
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
