<h1>Workshop - Multiplayer</h1>
<p>In this workshop we will discover how to make a game compatible for multiplayer using Fishnet. Fishnet is a library that handles all of the logic for what is needed for multiplayer. Beneath you see the steps to setup Fishnet / Tailscale. Good luck!</p>

<br>

## Fishnet

### 1. Installing Fishnet

- Clone this repository, add the project to unity and start it.
- Go to the <a href="https://assetstore.unity.com/packages/tools/network/fishnet-networking-evolved-207815">Asset Store</a> and click on 'Add to My Assets'. 
- Open in Unity and click on 'import ... to project'. 
- After downloading, click on import.
- Then go to **Assets > FishNet > Demos > Prefabs** and drag the NetworkManager into the scene:
<img width="1917" height="907" alt="Naamloos" src="https://github.com/user-attachments/assets/9501ed78-b083-4398-b88e-6f00c94d374d" />

- Expand the NetworkManager and click on NetworkHudCanvas. There, change the Auto Start Type to Host: 
<img width="1902" height="839" alt="Naamloos" src="https://github.com/user-attachments/assets/cbb81f79-60a4-4808-8ea3-76df8a487775" />



## Tailscale
### Logging into the account

- Go to tailscale.com and login using the google account that has been provided.
- Click on the download button and install Tailscale
- Make sure you are logged into Tailscale on the app
- Only one person has to find their peer in the list of IP addresses, their laptop name will be the best way to find their IP. (make sure you both are using the same IP address)
- Go to fishnet and open up the NetworkManager
- Find the Tugboat section of the NetworkManager and Change your client address fron "localhost" to the IP address.
<img width="468" height="456" alt="image" src="https://github.com/user-attachments/assets/c4b73a87-e33b-4829-b37a-a1282f3f6076" />


### Troubleshooting
Unable to find the laptop name?
- Open up CMD (Command Prompt)
- Enter "hostname"
- That is your laptop name




## Player Movement

Here we will adjust the existing script so that other players can see you move.

**Step 1:** Go to Scenes and open the 'Fishnet' scene

You now should see a world with some trees. This game is currently only for single player. You can run around, jump and swing your axe. After hitting a tree 5 times, it will drop a log. You can collect the log by pressing the interact key ('e'). You can also place a house by using the build key ('b'). We want to add multiplayer to this game. 

**Step 2:** Go to Scripts > Player > FirstPerson > FirstPersonController.cs

NetworkBehaviour is an interface available for objects that have the NetworkObject component. This interface provides functions to communicate with other players (observers) or the server. It also provides a check of whether you are the owner or not.

**Step 3:** Change the inheritance from MonoBehaviour to NetworkBehaviour. Make sure to have `using FishNet.Object;` among all the imports. 

As a client you would not want to see and register the camera of other players. If you do not disable the camera's of other players, a lot of errors will occur. We can disable this by checking if you are the owner. If not, disable the camera.

**Step 4:**  Add the following code to the script:

```csharp
public override void OnStartClient()
{
	if (!IsOwner)
	{
		enabled = false;
		if (playerCamera != null) playerCamera.gameObject.SetActive(false);
	}
}
```

**Step 5:** Open the player prefab and add the component "NetworkTransform". You can leave the default settings. This component updates the position of the object to other players.

You should now be able to see each other moving. It is normal to not see each others animations, as we will do this part next.

## Animations

We will now make sure that the animation variables are updated to everyone, and not only you.

**Step 1:** Open the player prefab and add the component "NetworkAnimator". You can leave the default settings. This component does **not** replace the "Animator" component. It only handles the synchronisation of the variables.

## Axe swinging

**Step 1:** Go to Scripts > Player > FirstPerson > PlayerCombat.cs

**Step 2:** Change the inheritance from MonoBehaviour to NetworkBehaviour.

**Step 3:** Add the following code to the script

```csharp
public override void OnStartClient()
{
	if (!IsOwner)
		enabled = false;
}
```

This code will make sure that when you swing your axe, other players don't swing their axe as well.

## Tree cutting

After hitting a tree 5 times, it drops a branch. This should also be 

## Building placement

