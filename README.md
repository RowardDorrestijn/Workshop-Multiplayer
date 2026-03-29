<img width="1902" height="839" alt="image" src="https://github.com/user-attachments/assets/e9b4f34f-fdbc-4d9c-9ac2-f23d33de96a7" /><h1>Workshop - Multiplayer</h1>
<p>In this workshop we will discover how to make a game compatible for multiplayer using Fishnet. Fishnet is a library that handles all of the logic for what is needed for multiplayer. Beneath you see the steps to setup Fishnet / Tailscale. Good luck!</p>

<br>

## Fishnet

### 1. Installing Fishnet

- Go to the <a href="https://assetstore.unity.com/packages/tools/network/fishnet-networking-evolved-207815">Asset Store</a> and click on 'Add to My Assets'. 
- Open in Unity and click on 'import ... to project'. 
- After downloading, click on import.
- Then go to **Assets > FishNet > Demos > Prefabs** and drag the NetworkManager into the scene:
<img width="1917" height="907" alt="Naamloos" src="https://github.com/user-attachments/assets/9501ed78-b083-4398-b88e-6f00c94d374d" />

- Expand the NetworkManager and click on NetworkHudCanvas. There, change the Auto Start Type to Host: 
<img width="1902" height="839" alt="Naamloos" src="https://github.com/user-attachments/assets/cbb81f79-60a4-4808-8ea3-76df8a487775" />



## Tailscale

### 1. Create an account

- Go to tailscale.com and login using one of the options provided.
- Fill in the form (does not matter what you say)
- Go to the next step
- Click on the download button and install Tailscale

### 2. Host

- Click on 'Skip this introduction' at the bottom of the page
- Go to the 'Users' tab and click on 'Invite users' outside of my organisation:
<img width="1167" height="365" alt="image" src="https://github.com/user-attachments/assets/8ce485cc-5550-411d-9e31-49bb9b1afa7c" />

- Copy the link and send it to your working partner

### 3. Client

- Click on the link
- Login using the account you just created
- It should give you the option to join your partners network, click on that one
- Open a command line
- Execute 'tailscale status'
- Check if you see both you and your partners machine


## Player Movement

Here we will adjust the existing script so that other players can see you move.

**Step 1:** Go to Scripts > Player > FirstPerson > FirstPersonController.cs

NetworkBehaviour is an interface available for objects that have the NetworkObject component. This interface provides functions to communicate with other players (observers) or the server. It also provides a check of whether you are the owner or not.

**Step 2:** Change the inheritance from MonoBehaviour to NetworkBehaviour. Make sure to have `using FishNet.Object;` among all the imports. 

As a client you would not want to see and register the camera of other players. If you do not disable the camera's of other players, a lot of errors will occur. We can disable this by checking if you are the owner. If not, disable the camera.

**Step 3:**  Add the following code to the script:

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

**Step 4:** Open the player prefab and add the component "NetworkTransform". You can leave the default settings. This component updates the position of the object to other players.

You should now be able to see each other moving. It is normal to not see each others animations, as we will do this part next.

## Player Combat
**Step 1:** Go to Scripts > Player > FirstPerson > PlayerCombat.cs
**Step 2:** Use NetworkBehaviour instead of MonoBehaviour and import `using FishNet.Object;` on the top of the script.
**Step 3:** Add the following code to the script
```csharp
public override void OnStartClient()
{
	if (!IsOwner)
		enabled = false;
}

[ObserversRpc]
private void SwingObserversRpc()
{
	animator.SetTrigger("Swing");
}

[ServerRpc]
private void SwingServerRpc()
{
	SwingObserversRpc();
}
```

Step 4: add `SwingServerRpc()` to the update function underneath `animator.SetTrigger("Swing")`.

