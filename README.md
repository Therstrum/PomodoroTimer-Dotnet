# PomodoroTimer-Dotnet
Console app for a pomodoro timer. With Spotify and HomeAssistant integration

This app can be used without Spotify or Home Assistant integration, just as a normal pomodoro timer.

# Spotify integration
To integrate spotify, you need a spotify account and developer account (developer account requirment may be removed in a later version)

Resources: https://developer.spotify.com/dashboard

# Home Assistant integration
To integrate Home Assistant, you need a Home Assistant instance running somewhere, a Home Assistant long lived access token, and two automations:

1. Automation to do something when the pomodoro timer starts a work session.
2. Automation to do something when the pomodoro timer ends a work session.

Resources: 
- https://www.home-assistant.io/docs/automation/
- https://www.home-assistant.io/docs/authentication/
