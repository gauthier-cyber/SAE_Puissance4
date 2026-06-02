using System;

namespace Puissance4.Systeme
{
    // ============================================================
    //  CLASSE RESERVEE POUR L'INTELLIGENCE ARTIFICIELLE
    // ============================================================
    //
    //  ATTENTION : cette classe n'est PAS encore developpee.
    //  On la fera plus tard, ensemble.
    //
    //  Voici ce qu'elle devra contenir quand on s'en occupera :
    //
    //  1) L'IA "Idiot" :
    //     - Elle choisira simplement une colonne au hasard parmi les
    //       colonnes encore jouables.
    //     - On utilisera la classe Random (vue en cours) pour tirer un
    //       numero de colonne, puis on verifiera avec plateau.ColonneJouable(...)
    //       que la colonne n'est pas pleine.
    //
    //  2) L'IA "Intelligent" :
    //     - Elle regardera si elle peut gagner en un coup.
    //     - Sinon, elle regardera si l'adversaire peut gagner au coup
    //       suivant, pour bloquer.
    //     - Sinon, elle jouera un coup correct (par exemple au centre).
    //
    //  La methode principale ressemblera a ceci (a coder plus tard) :
    //
    //     public int ChoisirColonne(Plateau plateau, int niveauIA, int numeroJoueurIA)
    //     {
    //         // TODO : ecrire ici le choix de la colonne selon le niveau.
    //         // Doit renvoyer un numero de colonne jouable.
    //         return 0;
    //     }
    //
    //  Rappel important : cette classe doit rester dans le back-end,
    //  donc PAS d'affichage ici (pas de MessageBox, pas de couleurs).
    //  Elle renvoie juste un numero de colonne (int) au front.
    //
    // ============================================================

    public class Ia
    {
        // Pour l'instant la classe est vide, on la remplira plus tard.
    }
}