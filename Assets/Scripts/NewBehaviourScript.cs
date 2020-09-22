using System.Collections.Generic;
using System.Collections;
using System;

using HomeIsland.mvu;
using SRC.Model;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    //Start is called before the first frame update
    private static Func<Msg, Model, (Model, IEnumerable<Cmd>)?> MVUUpd = new Updater
    {
      NotStarted =
      {
          StartInit = (msg, model) => (
              Model.normal(),
              new[] {Cmd.showWinScreen()})
      },
      Normal =
      {
          CompletedCasual = (msg, model) => (
              Model.normal(),
              new Cmd[] { })
      },
      
      NotNormal =
      {
          CompletedCasual = (msg, model) => (
              Model.normal(),
              new Cmd[] { })
      },
      
      Bbb =
      {
          CompletedCasual = (msg, model) => (
              Model.normal(),
              new Cmd[] { })
      },
      
      
      Ddd=
      {
          CompletedCasual = (msg, model) => (
              Model.normal(),
              new Cmd[] { })
      }
        }.handle;
       void Start()
       {
          var m = Model.notStarted();
          var m2 = MVUUpd(Msg.startInit(), m);

          var m3 = MVUUpd(Msg.loadedLocalSave(new MapState(1000, null, null)), m);
       }
    
}
