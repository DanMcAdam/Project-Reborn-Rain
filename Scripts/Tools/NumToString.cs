using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumToString
{
    private static string[] _intArray;
    private static int _defaultArraySize = 1000;
    public static string ReturnString(int integer)
    {
        if (_intArray == null)
        {
            _intArray = new string[_defaultArraySize];
            for(int i = 0; i < _defaultArraySize+1; i++)
            {
                _intArray[i] = i.ToString();
            }
        }
        if (integer > _intArray.Length)
        {
            _intArray = new string[integer];
            for (int i = 0; i < integer+1; i++)
            {
                _intArray[i] = i.ToString();
            }
        }
        return _intArray[integer];
        
    }
}
