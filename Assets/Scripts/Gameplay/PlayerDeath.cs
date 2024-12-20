﻿using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            model.virtualCamera.m_Follow = null;
            model.virtualCamera.m_LookAt = null;
            // player.collider.enabled = false;
            player.controlEnabled = false;

            player.animator.SetTrigger("hurt");
            player.animator.SetBool("dead", true);
            Debug.Log("Player Dead - from PlayerDeath.cs");
            Simulation.Schedule<PlayerSpawn>(2);
            
        }
    }
}