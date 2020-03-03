using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class KILL_THE_MOBS : PhysicsGame
{
    const double nopeus = 200;
    const double hyppyNopeus = 750;
    const int RUUDUN_KOKO = 40;

    PlatformCharacter pelaaja1;
    PlatformCharacter pelaaja2;
    IntMeter laskuri;
    PhysicsObject tahti2;
    PhysicsObject tahti;
    PhysicsObject nyancat;
    Weapon pelaajan1;
    Weapon pelaajan2;
    ExplosionSystem rajahdys;
    DoubleMeter elamaLaskuri;
    

    Image pelaajanKuva = LoadImage("steave");
    Image voitto = LoadImage("timantti");
    Image pelaajanKuva2 = LoadImage("steave2");
    Image tahtiKuva = LoadImage("elämä");
    Image tahtiKuva2 = LoadImage("creeper");
    Image tahtiKuva3 = LoadImage("nyan");
    Image taustaKuva1 = LoadImage("iron");
    Image taustakuva = LoadImage("gold");
    Image rajahdyskuva = LoadImage("rajahdys");
    Image miekka = LoadImage("miekka");

    SoundEffect maaliAani = LoadSoundEffect("maali");

    IntMeter LuoPisteLaskuri(Double x, Double y)
    {
        
        laskuri = new IntMeter(0);
        laskuri.MaxValue = 999;
        laskuri.MinValue = -999;

        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = x;
        naytto.Y = y;
        naytto.TextColor = Color.Maroon;
        naytto.BorderColor = Level.Background.Color;
        naytto.Color = Level.Background.Color;
        Add(naytto);

        return laskuri;
    }


    public override void Begin()
    {
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();
        LuoElamaLaskuri();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;

        
    }

    void LuoKentta()
    {
        MediaPlayer.Play("mario");
        TileMap kentta = TileMap.FromLevelAsset("kentta1");
        kentta.SetTileMethod('0', LisaaTaso2);
        kentta.SetTileMethod('2', LisaaTaso5);
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('1', LisaaPelaaja2);
        kentta.SetTileMethod('A', LisaaTahti2);
        kentta.SetTileMethod('5', LisaaTahti3);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.Image = null;
        //  Level.Background.TileToLevel();
        LuoPisteLaskuri(0, Screen.Bottom + 20);

        rajahdys = new ExplosionSystem(rajahdyskuva, 200);
        Add(rajahdys);

    }
    

    void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Image = taustaKuva1;
        Add(taso);
    }
      void LisaaTaso2(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Image = taustaKuva1;
        taso.IgnoresCollisionResponse = true;
        Add(taso);  
    }
        void LisaaTaso5(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Image = voitto;
        taso.IgnoresCollisionResponse = false;
        Add(taso);
    }

    void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void LisaaTahti2(Vector paikka, double leveys, double korkeus)
    {
        tahti2 = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti2.IgnoresCollisionResponse = false;
        tahti2.Position = paikka;
        tahti2.Image = tahtiKuva2;
        tahti2.Tag = "creeper";
        
        //Add(rajahdys);
        Add(tahti2);
    }
        void LisaaTahti3(Vector paikka, double leveys, double korkeus)
    {
        nyancat = new PhysicsObject(leveys, korkeus);
        nyancat.IgnoresCollisionResponse = false;
        nyancat.Position = paikka;
        nyancat.Image = tahtiKuva3;
        nyancat.Tag = "nyan";
        nyancat.Restitution = 1.0;
        Add(nyancat);
        RandomMoverBrain satunnaisAivot = new RandomMoverBrain(200);
        satunnaisAivot.ChangeMovementSeconds = 3;
        nyancat.Brain = satunnaisAivot;
    }
        
      
    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        Add(pelaaja1);
        AddCollisionHandler(pelaaja1, "creeper", TormaaTahteen2);
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja1, "nyan", TormaaTahteen3);

        pelaaja1.Weapon = new AssaultRifle(1, 1);
        pelaaja1.Weapon.Ammo.Value = 50;
        pelaaja1.Weapon.ProjectileCollision = AmmusOsui;
        pelaaja1.Weapon.Image = null;
        pelaajan1 = pelaaja1.Weapon;
        //pelaaja1.Add(pelaajan1);
    }

    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        if (kohde.Color == Color.Green || kohde.Image == taustaKuva1)
        {
            ammus.Destroy();
            return;
        }
        else
        {
            ammus.Destroy();
            kohde.Destroy();
        }

        
        
    }

    void LisaaPelaaja2(Vector paikka, double leveys, double korkeus)
    {
        pelaaja2 = new PlatformCharacter(leveys, korkeus);
        pelaaja2.Position = paikka;
        pelaaja2.Mass = 4.0;
        pelaaja2.Image = pelaajanKuva2;
        Add(pelaaja2);
        AddCollisionHandler(pelaaja2, "creeper", TormaaTahteen2);
        AddCollisionHandler(pelaaja2, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja2, "nyan", TormaaTahteen3);

        pelaaja2.Weapon = new AssaultRifle(1, 1);
        pelaaja2.Weapon.InfiniteAmmo = true;
        pelaaja2.Weapon.ProjectileCollision = AmmusOsui;
        pelaaja2.Weapon.Image = miekka;
        pelaajan2 = pelaaja2.Weapon;
    }

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja2, -nopeus);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja2, nopeus);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, hyppyNopeus);
        Keyboard.Listen(Key.Enter, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1);
        Keyboard.Listen(Key.CapsLock, ButtonState.Down, LyoMiekalla, "Ammu", pelaajan2);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }
    void AmmuAseella(Weapon ase)
    {
        PhysicsObject ammus = ase.Shoot();
    }

    void LyoMiekalla(Weapon miekkaa)
    {
        PhysicsObject miekkaAmmus = pelaaja2.Weapon.Shoot();
        if (miekkaAmmus != null)
        {
            miekkaAmmus.MaximumLifetime = TimeSpan.FromSeconds(0.02);
            miekkaAmmus.Image = miekka;
            miekkaAmmus.Size *= 2;
        }
    }

    void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("SAIT ELÄMÄN    :D");
        laskuri.Value += 1;
        elamaLaskuri.Value += 1;
        tahti.Destroy();
    }

    
    void TormaaTahteen2(PhysicsObject hahmo, PhysicsObject tahti2)
    {
        
        rajahdys.AddEffect(hahmo.X, hahmo.Y, 50);

        maaliAani.Play();
        MessageDisplay.Add("Otit Osuman   D:");
        laskuri.Value -= 1;
        elamaLaskuri.Value -= 3;
        tahti2.Destroy();
    }

    void TormaaTahteen3(PhysicsObject hahmo, PhysicsObject tahti3)
    {
        maaliAani.Play();
        MessageDisplay.Add("Sateenkaari Tauti   D:");
        laskuri.Value -= 5;
        elamaLaskuri.Value -= 5;
        tahti3.Destroy();
    }

        void LuoElamaLaskuri()
    {
        elamaLaskuri = new DoubleMeter(10);
        elamaLaskuri.MaxValue = 10;
        elamaLaskuri.LowerLimit += ElamaLoppui;

        ProgressBar elamaPalkki = new ProgressBar(150, 20);
        elamaPalkki.X = Screen.Left + 500;
        elamaPalkki.Y = Screen.Top - 50;
        elamaPalkki.BindTo(elamaLaskuri);
        Add(elamaPalkki);
    }
        void ElamaLoppui()
    {
        MessageDisplay.Add("KUOLIT! Yritä uudelleen D:");
    }
    
}