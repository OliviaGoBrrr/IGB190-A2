The ScreenFade class provides some basic functionality for fading the screen. The script logic is in the 'DeveloperToolbox' namespace. 
Either add a 'using DeveloperToolbox;' include in your script, or reference the namespace explicitly (like below).

No setup is required for this to work. The default fade durations are one second, and the complete actions are not required.

Fade In (Black->Game): Call "DeveloperToolbox.ScreenFade.FadeIn(duration, optionalCompleteAction);"
Fade Out (Game->Black): Call "DeveloperToolbox.ScreenFade.FadeOut(duration, optionalCompleteAction);"
Fade Out and In (Game->Black->Game): Call "DeveloperToolbox.ScreenFade.Fade(fadeOutDuration, holdDuration, fadeInDuration, fadedOutAction);"

To check if a screen fade is in progress: DeveloperToolbox.ScreenFade.IsFading()
To stop a screen fade: DeveloperToolbox.ScreenFade.Stop()