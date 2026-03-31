<h1>Workshop - Multiplayer</h1>
<p>In this workshop we will discover how to make a game compatible for multiplayer using Fishnet. Fishnet is a library that handles all of the logic for what is needed for multiplayer. Beneath you see the steps to setup Fishnet / Tailscale. Good luck!</p>

<br>

## Fishnet

### 1. Installing Fishnet

- Clone this repository, add the project to unity and start it. If you do not have the same version, use one you already have installed.
- Go to Scenes and open the Fishnet scene.
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

You now should see a world with some trees. This game is currently only for single player. You can run around, jump and swing your axe. After hitting a tree 5 times, it will drop a log. You can collect the log by pressing the interact key ('e'). You can also place a house by using the build key ('b'). We want to add multiplayer to this game.

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

**Step 4:** Open the Player prefab and add the component "NetworkTransform". You can leave the default settings. This component updates the position of the object to other players.

You should now be able to see each other moving. It is normal to not see each others animations, as we will do this part next.

## Animations

We will now make sure that the animation variables are updated to everyone, and not only you.

**Step 1:** Open the Player prefab and add the component "NetworkAnimator". You can leave the default settings. This component does **not** replace the "Animator" component. It only handles the synchronisation of the variables.

## Axe swinging

**Step 1:** Go to Scripts > Player > FirstPerson > PlayerCombat.cs

**Step 2:** Change the inheritance from MonoBehaviour to NetworkBehaviour.

**Step 3:** Put the following line at the top of CheckSlash()

```csharp
if (!IsOwner) return;
```

This line will make sure that when you execute this function, it won't be executed on other clients as well.

## Tree cutting

In this game, you can get wood by cutting trees. After hitting a tree 5 times, it drops a branch. We will now learn how to synchronize variables so that you can see someone else cutting a tree.

**Step 1:** Go to Prefabs > Nature > Tree_(0-5) and for each of these trees add a NetworkObject component. Disable **Is Spawnable**.

**Step 2:** Go to Scripts > Interactables and open TreeController.cs. There, change the inheritance from MonoBehaviour to NetworkBehaviour.

**Step 3:** Replace this line:

```csharp
private int currentAmountOfHitsNeededForBranch;
```

with

```csharp
private readonly SyncVar<int> currentAmountOfHitsNeededForBranch = new SyncVar<int>();
```

**Step 4:** Remove this line from Awake():

```csharp
currentAmountOfHitsNeededForBranch = amountOfHitsNeededForBranch;
```

Problem now is that we can't directly assign values to currentAmountOfHitsNeededForBranch.

**Step 5:** Add this function to the code:

```csharp
public override void OnStartServer()
{
    currentAmountOfHitsNeededForBranch.Value = amountOfHitsNeededForBranch;
}
```

**Step 6:** Replace the Hit() function with this:

```csharp
[ServerRpc(RequireOwnership = false)]
public void Hit(Transform playerTransform)
{
    if (!isShaking)
    {
        currentAmountOfHitsNeededForBranch.Value--;

        if (currentAmountOfHitsNeededForBranch.Value <= 0)
        {
            DropBranch(playerTransform);
            currentAmountOfHitsNeededForBranch.Value = amountOfHitsNeededForBranch;
        }
    }
}
```

By adding ```[ServerRpc]``` to a function you, as a client, can call this function to run on the server. All objects placed in the editor in the scene are owned by the server. By adding ```(RequireOwnership = false)```, we can hit the tree without owning it. If you do not add this line, you are not able to hit the tree.

Notice how we deleted ```StartCoroutine(Shake());```. We did this because shaking should be visible to everyone. This function now is handled on the server side. 

**Step 7:** Add the following function:

```csharp
private void OnHitsChanged(int prev, int next, bool asServer)
{
    StartCoroutine(Shake());
}
```

**Step 8:** Add the following line to Awake():

```csharp
currentAmountOfHitsNeededForBranch.OnChange += OnHitsChanged;
```

These OnChange functions will execute on every client, every time a syncvar changes. Do not forget to add the parameters ```(int prev, int next, bool asServer)``` to the OnChange function.

**Step 9:** Go to Prefabs > Nature and open the prefab Branch_01. Add a NetworkObject component. **Is Spawnable** must be turned on.

Now you are able to see someone else hitting a tree (shaking) and after 5 hits you will both see a branch spawning. You can pick up a branch by looking at it and pressing 'e'.

## Building placement

By pressing 'b' you are able to place a building. When the ground is good enough for building a house, the preview will turn green. If there are obstacles hindering the placement, it will turn red. By clicking with your mouse, you can place the building. We want this building to be visible to all other players.

**Step 1:** Go to Prefabs > Buildings and open the House_01_LITE prefab. Add a NetworkObject component. **Is Spawnable** must be turned on.

**Step 2:** Go to Scripts > Player and open PlaceBuildingManager.cs. Change the inheritance from MonoBehaviour to NetworkBehaviour.

You do not want to see what the preview of other players when they are placing a building.

**Step 3:** Put the following line at the top of the Update() function:

```csharp
if(!IsOwner) return;
```

**Step 4:** Scroll down until you see PlaceBuilding(). Replace this function with the following two functions:

```csharp
void PlaceBuilding()
{
    if (isBuildingMode && canPlace)
    {
        PlaceBuildingServerRpc(currentGhost.transform.position, currentGhost.transform.rotation);
        ToggleBuildMode();
    }
}

[ServerRpc]
private void PlaceBuildingServerRpc(Vector3 position, Quaternion rotation)
{
    GameObject house = Instantiate(buildingPrefab, position, rotation);
    Spawn(house);
}
```

PlaceBuildingServerRpc() is now executed on the server. Here we can spawn the house so that it is visible for everyone. ToggleBuildMode() is a function that you want to execute on your own client. Therefore it stays in PlaceBuilding().


## Interactables

Interactables are objects that highlight when you look at them. In this game we have two: branches and doors. When picking up a branch, you want it to be deleted for everyone. When opening a door you want it to be open for everyone.

**Step 1:** Go to Scripts > Interactables and open InteractableObject.cs. Change the inheritance from MonoBehaviour to NetworkBehaviour.

When you look at an interactable object, it gets highlighted. Since this is client side only, we don't need to change anything else to this script.

**Step 2:** Go to Scripts > Player and open PlayerInteraction.cs. Again replace the inheritance from MonoBehaviour to NetworkBehaviour.

**Step 3:** Replace the first if-statement with the following line:

```csharp
if (IsOwner && looker.TryGetLookedAt(out RaycastHit hit))
``` 

This line prevents the same thing as with slashing your axe. Now if someone presses 'e', it does not execute this code on every client.

**Step 4:** Go to Scripts > Interactables > Trees and open BranchController.cs. Add the following line one line above ```Destroy(gameobject)```:

```csharp
Despawn(gameObject);
```

Despawning an object updates for every client. It is as simple as this.

**Step 5:** Go to Scripts > Interactables > Doors and open DoorController.cs. Replace Interact() with:

```csharp
[ServerRpc(RequireOwnership = false)]
public override void Interact(GameObject player)
{
    if (!isMoving)
    {
        InteractObserversRpc();
    }
}

[ObserversRpc]
private void InteractObserversRpc()
{
    StartCoroutine(RotateDoor());
}
```

Again we used ```[ServerRpc(RequireOwnership = false)]```. The door is not owned by the player, but by the server. That is why ```RequireOwnership = false```. Now Interact() is a function that is handled on the server. Instead of a SyncVar<>, we use [ObserversRpc]. This is a different way to execute code on all clients. In RotateDoor the boolean 'isOpen' gets updated, therefore the variable gets updated on every client.

There is a problem with this solution however. You can spot this mistake by doing the following steps:

- Let the host start the game
- Place a building and open the door
- Let the other one join

You will now see that on the host the door is open and on the client the door is closed. This is the difference between SyncVar<> and [ObserversRpc]. SyncVar<> synchronizes and stores the value, also for late joiners. [ObserverRpc] only executes the code for the clients that are currently online. Use [ObserverRpc] for temporary effects (like firework, particles etc.). Use SyncVar<> for states that can be permanent (like the amounts needed to hit the tree).

## The end
