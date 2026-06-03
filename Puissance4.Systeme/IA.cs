using System;

namespace Puissance4.Systeme
{
    // ========================================================================
    //  CLASSE IA  ->  A DEVELOPPER PLUS TARD (NE PAS CODER POUR L'INSTANT)
    // ========================================================================
    //
    // C'est ici que toute la logique de l'ordinateur (l'IA) devra etre placee.
    // Pour l'instant on laisse seulement le squelette et les explications.
    // On developpera le contenu des methodes dans une prochaine seance.
    //
    // Rappel des deux niveaux prevus dans la maquette :
    //
    //   - IA "Idiot"      : elle doit juste choisir une colonne AU HASARD
    //                       parmi les colonnes encore jouables.
    //                       Astuce : la classe Plateau a deja une methode
    //                       ColonnesJouables() qui renvoie la liste des colonnes
    //                       valides. Il suffira de tirer un index au hasard
    //                       avec la classe Random et de renvoyer cette colonne.
    //
    //   - IA "Intelligent": elle devra reflechir (regarder si elle peut gagner,
    //                       bloquer l'adversaire, etc.). A faire plus tard.
    //
    // Idee de structure pour quand on la codera :
    //
    // public class IA
    // {
    //     private NiveauIA niveau;        // Idiot ou Intelligent
    //     private Random generateur;      // pour tirer un coup au hasard
    //
    //     public IA(NiveauIA niveau)
    //     {
    //         this.niveau = niveau;
    //         this.generateur = new Random();
    //     }
    //
    //     // Renvoie le numero de colonne choisi par l'IA pour le plateau donne.
    //     public int ChoisirColonne(Plateau plateau)
    //     {
    //         if (niveau == NiveauIA.Idiot)
    //         {
    //             // TODO : recuperer les colonnes jouables et en tirer une au hasard.
    //         }
    //         else
    //         {
    //             // TODO : logique de l'IA intelligente (plus tard).
    //         }
    //
    //         return 0; // valeur provisoire
    //     }
    // }
    //
    // ========================================================================
}