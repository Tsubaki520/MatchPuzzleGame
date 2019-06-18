﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Orbs.Canvas
{

    /// <summary>
    /// UI class for controlling the HP bar
    /// </summary>
    public class HealthBar : MonoBehaviour
    {

        /// <summary>
        /// Allow static access to the health bar
        /// </summary>
        public static HealthBar instance;

        /// <summary>
        /// Reference to the SimpleHealthBar from the plugin
        /// </summary>
        public SimpleHealthBar healthBar;
        /// <summary>
        /// Event raised when the maxTime is reached in timer mode
        /// </summary>
        public event EventHandler TimeReached;

        /// <summary>
        /// Current health
        /// </summary>
        private float health = 30;
        /// <summary>
        /// Maximum health
        /// </summary>
        private float maxHealth = 30;
        /// <summary>
        /// Boolean storing whether the health bar is displaying countdown timer
        /// </summary>
        private bool timerMode = false;
        /// <summary>
        /// Time elapsed since the countdown begin
        /// </summary>
        private float timeElapsed = 0;
        /// <summary>
        /// Boolean tracking if prolong time skill is activated
        /// </summary>
        private bool timeSkillActivated = false;

        /// <summary>
        /// Health Bar Color for upper 1/3 of health
        /// </summary>
        private readonly Color high = Color.green;
        /// <summary>
        /// Health Bar Color for middle 1/3 of health
        /// </summary>
        private readonly Color mid = Color.yellow;
        /// <summary>
        /// Health Bar Color for lower 1/3 of health
        /// </summary>
        private readonly Color low = Color.red;
        /// <summary>
        /// Maximum time allowed in timer mode (4 seconds for movement)
        /// </summary>
        private readonly float maxTime = 4;
        /// <summary>
        /// Maximum time allowed in timer mode but with skill activated (12 seconds for movement)
        /// </summary>
        private readonly float maxTimeSkill = 24;

        /// <summary>
        /// Initiailize the health bar to the set maxHealth
        /// </summary>
        public void Start ()
        {
            // Initialize variables
            instance = this;
            // Get maximum health
            maxHealth = Coordinator.StageManager.instance.getMaxRound ();
            health = maxHealth;
            // Update health bar
            healthBar.UpdateBar (health, maxHealth);
        }

        /// <summary>
        /// Executed every frame
        /// </summary>
        public void Update ()
        {
            if (timerMode)
            {
                // Elapse time if in timer mode
                timeElapsed += Time.deltaTime;
                if ((!timeSkillActivated && timeElapsed <= maxTime) || (timeSkillActivated && timeElapsed <= maxTimeSkill))
                {
                    // Update timer if not reach maximum yet
                    if (timeSkillActivated)
                    {
                        healthBar.UpdateBar (maxTimeSkill - timeElapsed, maxTimeSkill);
                    }
                    else
                    {
                        healthBar.UpdateBar (maxTime - timeElapsed, maxTime);
                    }
                }
                else
                {
                    // Maximum allowed time reached
                    // Disable timer mode and restore health bar
                    timerMode = false;
                    healthBar.UpdateBar (health, maxHealth);
                    // Disable prolong timer mode
                    timeSkillActivated = false;
                    // Restore the text
                    healthBar.UpdateTextColor (Color.white);
                    // Raise TimeReached event
                    OnTimeReached (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Request the next movement to have prolonged movement time to 12 seconds
        /// </summary>
        public void RequestProlong ()
        {
            timeSkillActivated = true;
        }

        /// <summary>
        /// Request this health bar to display in countdown mode and begin count down
        /// </summary>
        public void RequestCountdown ()
        {
            // Enable timer mode
            timerMode = true;
            timeElapsed = 0;
            // Set the health bar
            healthBar.UpdateBar (maxTime - timeElapsed, maxTime);
            // Not display the text
            healthBar.UpdateTextColor (new Color (0, 0, 0, 0));
        }

        /// <summary>
        /// Stop the timer mode forcefully
        /// </summary>
        public void StopCountdown ()
        {
            if (timerMode)
            {
                // Disable prolong timer mode
                timeSkillActivated = false;
                // Shutdown timer mode and restore health bar if timerMode is activated
                timerMode = false;
                healthBar.UpdateBar (health, maxHealth);
                TimeReached = null;
                // Restore the text
                healthBar.UpdateTextColor (Color.white);
            }
        }

        /// <summary>
        /// Call this method whenever a round end to reduce 1 HP and update the color of the health bar
        /// </summary>
        /// <returns>Trye if all round has been used and the game should be ended</returns>
        public bool OnRoundEnded ()
        {
            health -= 1;
            // Update health bar
            healthBar.UpdateBar (health, maxHealth);
            // Update color
            if (health / maxHealth >= 0.666f)
            {
                healthBar.UpdateColor (high);
            }
            else if (health / maxHealth >= 0.333f)
            {
                healthBar.UpdateColor (mid);
            }
            else
            {
                healthBar.UpdateColor (low);
            }
            return health <= 0;
        }

        /// <summary>
        /// Raise TimeReached event once the animation is completed
        /// </summary>
        /// <param name="e">Empty event arguments</param>
        public virtual void OnTimeReached (EventArgs e)
        {
            EventHandler handler = TimeReached;
            if (handler != null)
            {
                handler (this, e);
            }
            TimeReached = null;
        }

    }

}
