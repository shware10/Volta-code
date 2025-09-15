using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    void OnStateChanged(GameState state);
}
