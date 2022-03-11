/// <summary>
/// Contains method cotracts to 
/// be used to read and write 
/// GameData to a file. 
/// </summary>
public interface IDataHandler 
{
    /// <summary>
    /// Implement this to retrieves the game data through a
    /// storage mechanism.
    /// </summary>
    /// <returns>A copy of the game data</returns>
    GameData Load();

    /// <summary>
    /// Implement this to save the game data through a
    /// storage mechanism.
    /// </summary>
    /// <param name="gameData">A copy of the game data</param>
    void Save(GameData gameData);
}
