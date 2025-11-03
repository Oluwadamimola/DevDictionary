# DevDictionary Bot

An AI-powered developer dictionary bot for Telex.im that provides instant definitions and explanations of programming terms, technologies, and concepts.

##  What It Does

DevDictionary acts like a tech-savvy teammate on Telex. When someone asks about a programming term, the bot fetches clear explanations from trusted sources like MDN Web Docs and Wikipedia, delivering answers right in the chat.

##  Features

- **Instant Lookups**: Quick definitions for any programming term or tech concept
- **Multiple Sources**: Searches MDN Web Docs and Wikipedia for comprehensive results
- **Natural Language**: Understands questions like "What is REST?" or "Explain Docker"
- **Clean Responses**: Returns well-formatted, easy-to-read answers with source attribution
- **Error Handling**: Gracefully handles unknown terms with helpful suggestions

##  Tech Stack

- **Backend**: .NET 8 (C#)
- **Integration**: Telex.im REST API + Webhooks
- **Data Sources**: 
  - MDN Web Docs API
  - Wikipedia REST API
- **Deployment**: Railway (Docker container)

##  Prerequisites

- .NET 8.0 SDK
- Telex.im account with API access
- Railway account (for deployment)

##  Quick Start

### Local Development

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd DevDictionary
   ```

2. **Restore dependencies:
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Test the endpoints:**
   - Health check: `http://localhost:5000/telex/health`
   - Webhook: `POST http://localhost:5000/telex/webhook`

```

##  Telex Integration


###  Test the Bot

Send a message in any Telex channel:

```
User: What is REST API?
Bot:  REST API
Representational State Transfer (REST) is an architectural style...
_Source: Wikipedia_
🔗 https://en.wikipedia.org/wiki/REST
```

## 📡 API Endpoints

### POST /telex/webhook
Receives webhook events from Telex.im

**Request Body:**
```json
{
  "type": "message",
  "message": {
    "id": "msg_123",
    "text": "What is Docker?",
    "timestamp": 1699564800
  },
  "channel": {
    "id": "channel_abc",
    "name": "general"
  },
  "user": {
    "id": "user_xyz",
    "username": "developer",
    "name": "John Doe"
  }
}
```

**Response:** 200 OK (bot sends reply asynchronously)

### GET /telex/health
Health check endpoint

**Response:**
```json
{
  "status": "healthy",
  "service": "DevDictionary Bot",
  "timestamp": "2025-01-29T12:00:00Z"
}
```

## 🎮 Usage Examples

The bot understands natural language queries:

```
✅ "What is middleware?"
✅ "Explain async/await"
✅ "Define REST API"
✅ "What's Docker?"
✅ "Meaning of CI/CD"
```



## 🐛 Troubleshooting

### Bot doesn't respond
- Check Railway logs for errors
- Verify webhook URL is correct in Telex settings

### "Definition not found" errors
- The term might not exist in MDN or Wikipedia
- Try alternative phrasing
- Check logs to see which APIs were queried

### Webhook failures
- Ensure your Railway app is running
- Check that PORT environment variable is set correctly
- Verify CORS is enabled for Telex domain

##  Development Notes

- **No database required**: All operations are stateless
- **Real-time responses**: Bot fetches data on-demand
- **Fallback sources**: Tries MDN first, then Wikipedia
- **Error resilient**: Returns friendly messages for failed lookups
