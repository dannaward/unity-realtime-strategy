using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;

    public static event Action<Unit> ServerOnUnitSpawned; 
    public static event Action<Unit> ServerOnUnitDespawned; 
    
    public static event Action<Unit> AuthorityOnUnitSpawned; 
    public static event Action<Unit> AuthorityOnUnitDespawned; 

    public int GetResourceCost()
    {
        return resourceCost;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;

        ServerOnUnitDespawned?.Invoke(this);
    }
    
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeSelected?.Invoke();
    }
    
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned.Invoke(this);
    }
    
    public override void OnStopClient()
    {
        AuthorityOnUnitDespawned.Invoke(this);
    }
    
    #endregion
}
