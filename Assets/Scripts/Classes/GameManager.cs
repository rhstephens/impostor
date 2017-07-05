
/// <summary>
/// The game manager that players will interact with for client-side tasks
/// </summary>
public class GameManager : Singleton<GameManager> {

	
	public IWeapon GetPlayerWeapon() {
        //TODO: Make this smarter... much smarter
        return new DesertEagle();
    }
}
