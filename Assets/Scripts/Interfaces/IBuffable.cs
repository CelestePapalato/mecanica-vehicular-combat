using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    public void Accept(IBuff buff);
}