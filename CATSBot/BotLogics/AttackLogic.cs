﻿using System;
using System.Drawing;
using System.Threading;

using CATSBot.Helper;

namespace CATSBot.BotLogics
{
    public static class AttackLogic
    {
        static Point pNull = new Point(0, 0);
        static Random rnd = new Random();

        //This is disabled until re-implemented. ;)
        static int wins = 0;
        static int losses = 0;
        static int winsInARow = 0;
        static int crowns = 0;

        //Check if we defended, if yes, click that filthy "Claim" button that's prevent us from clicking "QUICK FIGHT" ;)
        public static void checkDefense()
        {
            Helper.BotHelper.Log("Successful defense check");

            if (ImageRecognition.getPictureLocation(Properties.Resources.button_defend, BotHelper.memu) != pNull)
            {
                ClickOnPointTool.ClickOnPoint(BotHelper.memu, ImageRecognition.getRandomLoc(Properties.Resources.button_defend, BotHelper.memu));
                BotHelper.Log("Yup, we defended");
                BotHelper.randomDelay(1000, 100);
            }
        }

        // Try to find the "Quick Fight" button and click on it.
        public static bool searchDuell()
        {
            BotHelper.Log("Attempting to press the Duell button");
            if (ImageRecognition.getPictureLocation(Properties.Resources.button_fight, BotHelper.memu) != pNull)
            {
                Point dbgPoint = ImageRecognition.getPictureLocation(Properties.Resources.button_fight, BotHelper.memu);
                BotHelper.Log("Button found! FeelsGoodMan.");
                BotHelper.Log("Button found at: X = " + dbgPoint.X + "; Y = " + dbgPoint.Y, true, true);
                ClickOnPointTool.ClickOnPoint(BotHelper.memu, ImageRecognition.getRandomLoc(Properties.Resources.button_fight, BotHelper.memu));
                return true;
            }
            else
            {
                BotHelper.Log("Button not found! FeelsBadMan.");
                return false;
            }
        }

        //Check for the skip button. If it's there, an opponent has been found.
        public static bool waitDuell()
        {
            BotHelper.Log("Waiting for the duell to start....");
            int checks = 0;
            do
            {
                BotHelper.Log(" " + checks, false);
                Thread.Sleep(100);
                checks++;
            } while (ImageRecognition.getPictureLocation(Properties.Resources.button_skip, BotHelper.memu) == pNull && checks <= 55);

            if (checks >= 55)
            {
                BotHelper.Log("Oops, we timed out.");
                return false;
            }

            BotHelper.randomDelay(500, 50);
            return true;
        }

        // Start the fight by clicking anywhere and wait for it to end (by searching for the "OK" button)
        public static bool startDuell(int attempt = 1)
        {
            ClickOnPointTool.ClickOnPoint(BotHelper.memu, new Point(rnd.Next(670 - 100, 670 + 100), rnd.Next(400 - 100, 400 + 100))); //Click anywhere to start the battle
            BotHelper.randomDelay(500, 50);

            // wait for the duell to end and click on ok
            BotHelper.Log("Waiting for the duell to end.");
            int checks = 0;
            Point locOK = new Point();
            do
            {
                BotHelper.Log(" " + checks, false);
                Thread.Sleep(500);
                checks++;

                // Apparently, there are multiple "OK"-Buttons that all look the same at a first glance,
                // but there's a difference in them that the tool is able to detect. 
                // We have to check multiple images because of this, but we got an easy detection whether 
                // we won or not. :) 

                locOK = ImageRecognition.getPictureLocation(Properties.Resources.button_ok, BotHelper.memu);
                //locOKDefeat = ImageRecognition.getPictureLocation(Properties.Resources.button_ok_defeat, BotHelper.memu);
            } while ((locOK == pNull /* && locOKDefeat == pNull */) && checks <= 55);

            if (checks >= 55)
            {
                BotHelper.Log("We timed out. :(");
            }

            BotHelper.randomDelay(500, 50);
            if (locOK.X == 0 && locOK.Y == 0) //something went wrong (note: the code should never enter this)
            {
                BotHelper.Log("Something weird happened.");
                if (attempt < 3)
                    return startDuell(attempt + 1);
                else
                    return false; // TODO: Restart CATS?
            }
            else //we won!
            {
                BotHelper.Log("Battle finished.");

                Point rndP = ImageRecognition.getRandomLoc(locOK, Properties.Resources.button_ok);
                BotHelper.Log("Clicked on: X = " + rndP.X + "; Y = " + rndP.Y, true, true);
                ClickOnPointTool.ClickOnPoint(BotHelper.memu, rndP);
            }

            //BotHelper.UpdateStats(wins, losses, crowns);
            BotHelper.Log("Returning to main screen");

            return true;
        }

        //Attack Logic
        public static void doLogic()
        {
            BotHelper.randomDelay(4000, 1000);
            checkDefense();
            if (searchDuell())
            {
                if (waitDuell())
                {
                    if (startDuell())
                    {
                        BotHelper.Log("AttackLogic successfully completed.");
                    }
                    else
                    {
                        BotHelper.Log("AttackLogic failed during startDuell");
                    }
                }
                else
                {
                    BotHelper.Log("AttackLogic failed during waitDUell");
                }
            }           
            else
            {
                BotHelper.Log("AttackLogic failed during searchDuell");
            }
        }
    }
}
