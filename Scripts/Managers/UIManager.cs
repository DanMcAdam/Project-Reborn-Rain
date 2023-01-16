using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameplayUI GameplayUI;
    public InteractionUI InteractionUI;
    #region Singleton
    public static UIManager Instance;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private string[] _intArray;
    private int _defaultArraySize = 1000;
    public string ReturnString(int integer)
    {
        integer = Mathf.Abs(integer);
        if (_intArray is null)
        {
            _intArray = new string[_defaultArraySize + 1];
            for (int i = 0; i < _defaultArraySize + 1; i++)
            {
                _intArray[i] = i.ToString();
            }
        }
        else if (integer > _intArray.Length)
        {
            _intArray = new string[integer + 1];
            for (int i = 0; i < integer + 1; i++)
            {
                _intArray[i] = i.ToString();
            }
        }
        return _intArray[integer];

    }
}
