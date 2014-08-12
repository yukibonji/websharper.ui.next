﻿namespace IntelliFactory.WebSharper.UI.Next

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Dom
open IntelliFactory.WebSharper.UI.Next
open IntelliFactory.WebSharper.UI.Next.Notation

[<JavaScript>]
module Input =

    type MousePosSt =
        {
            mutable Active : bool
            PosV : Var<int * int>
        }

    type MouseBtnSt =
        {
            mutable Active : bool
            Left : Var<bool>
            Middle : Var<bool>
            Right : Var<bool>
        }

    let MousePosSt = { Active = false; PosV = Var.Create (0, 0) }
    let MouseBtnSt =
        {
            Active = false;
            Left = Var.Create false
            Middle = Var.Create false
            Right = Var.Create false
        }

    let ActivateButtonListener =
        let buttonListener (evt: Dom.MouseEvent) down =
            match evt.Button with
            | 0 -> Var.Set MouseBtnSt.Left down
            | 1 -> Var.Set MouseBtnSt.Middle down
            | 2 -> Var.Set MouseBtnSt.Right down
            | _ -> ()

        if not MouseBtnSt.Active then
            MouseBtnSt.Active <- true
            Dom.Document.Current.AddEventListener("mousedown",
                (fun (evt: DomEvent) -> buttonListener (evt :?> MouseEvent) true), false)
            Dom.Document.Current.AddEventListener("mouseup",
                (fun (evt: DomEvent) -> buttonListener (evt :?> MouseEvent) false), false)

    [<Sealed>]
    type Mouse =

        static member Position =

            let onMouseMove (evt: Dom.Event) =
                let mEvt = evt :?> Dom.MouseEvent
                // Update the RVars for the X and Y positions, from the information
                // contained within the event.
                Var.Set MousePosSt.PosV (mEvt.ClientX, mEvt.ClientY)

            if not MousePosSt.Active then
                Dom.Document.Current.AddEventListener("mousemove", onMouseMove, false)
                MousePosSt.Active <- true

            View.FromVar MousePosSt.PosV

        static member LeftPressed =
            ActivateButtonListener
            MouseBtnSt.Left.View

        static member MiddlePressed =
            ActivateButtonListener
            MouseBtnSt.Middle.View

        static member RightPressed =
            ActivateButtonListener
            MouseBtnSt.Right.View

        static member MousePressed =
            ActivateButtonListener
            View.Const (fun l m r -> l || m || r)
            <*> MouseBtnSt.Left.View
            <*> MouseBtnSt.Middle.View
            <*> MouseBtnSt.Right.View

    type Key = int

(*
    let keysPressed = Var.Create []
    let ActivateKeyListener (keyEvt: Dom.Ke) =
        keyEvt

    [<Sealed>]
    type Keyboard =

        static member KeysPressed =
            ActivateKeyListener
            *)