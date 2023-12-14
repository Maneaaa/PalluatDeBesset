void Main()
{

    Console.WriteLine(new string('-', 30));
    Console.WriteLine("        XMAS TREATS");
    Console.WriteLine(new string('-', 30));

    // Demander si le joueur a besoin d'aide
    Console.WriteLine("Avez-vous besoin d'aide ? (1 pour oui, 0 pour non) :");
    string? besoinAide = Console.ReadLine();

    if (besoinAide != null && besoinAide == "1")
    {
        AfficherAide();
    }


    int meilleurScore = 0;
    bool premierePartie = true;

    while (true)
    {
        char[,] grille = CreerEtInitialiserGrille();
        int nombreDeCoups = DemanderNombreDeCoups(); // Nombre total de coups autorisés
        int coupsJoues = 0;
        int coupsRestants = 0;

        bool finDePartie = false;
        do
        {
            coupsRestants = nombreDeCoups - coupsJoues; // Calcul des coups restants
            AfficherGrilleEtInfos(grille, coupsRestants, coupsJoues);

            if (GererMouvements(grille))
            {
                coupsJoues++;
            }

            finDePartie = VerifierFinDePartie(grille, nombreDeCoups, coupsJoues);

        } while (!finDePartie);
        AfficherGrilleEtInfos(grille, nombreDeCoups, coupsJoues);

        if (coupsJoues >= nombreDeCoups)
        {
            Console.WriteLine("Nombre de coups maximal atteint. Fin de la partie.");
        }

        // Calcul et affichage du score
        int score = CalculerScore(grille);
        Console.WriteLine($"Votre score : {score}");

        // Mise à jour du meilleur score
        if (premierePartie || score > meilleurScore)
        {
            meilleurScore = score;
            premierePartie = false;
        }

        // Affichage du meilleur score
        Console.WriteLine(premierePartie ? "Record actuel : Disponible à la fin de la première partie" : $"Meilleur score : {meilleurScore}");

        // Demander si le joueur veut rejouer
        Console.WriteLine("Voulez-vous rejouer ? Entrez 1 pour oui, 0 pour non :");
        string? reponse = Console.ReadLine(); // reponse est maintenant un string?

        // Vérifier si reponse est null avant de comparer sa valeur
        if (reponse != "1")
        {
            break; // Sortie de la boucle si le joueur ne souhaite pas rejouer
        }
    }
}

Main();

void AfficherAide()
{
    Console.WriteLine("\nBienvenue dans XMAS TREATS !");
    Console.WriteLine("Objectif : Rassemblez des bonbons pour marquer des points.");
    Console.WriteLine("Règles :");
    Console.WriteLine(" - Déplacez les bonbons pour les faire fusionner deux à deux.");
    Console.WriteLine(" - Chaque bonbon a une valeur de points différente.");
    Console.WriteLine(" - Vous avez un nombre limité de mouvements pour atteindre le meilleur score.");
    Console.WriteLine("\nCompte des Points :");
    Console.WriteLine(" - Bonbon '*' : 1 point");
    Console.WriteLine(" - Bonbon '@' : 3 points");
    Console.WriteLine(" - Bonbon 'o' : 7 points");
    Console.WriteLine(" - Bonbon 'J' : 15 points");
    Console.WriteLine("\nCommandes :");
    Console.WriteLine(" - 8 pour haut ⬆");
    Console.WriteLine(" - 6 pour droite ➡");
    Console.WriteLine(" - 4 pour gauche ⬅");
    Console.WriteLine(" - 2 pour bas ⬇");
    Console.WriteLine("\nBonne chance !\n");
}

// Crée et initialise une grille de jeu 4x4
char[,] CreerEtInitialiserGrille()
{
    int taille = 4;
    char[,] grille = new char[taille, taille];

    for (int i = 0; i < taille; i++)
    {
        for (int j = 0; j < taille; j++)
        {
            grille[i, j] = ' '; // Initialisation avec des espaces (cases vides)
        }
    }

    // Placer deux bonbons aléatoires au début du jeu
    AjouterBonbonAleatoire(grille);
    AjouterBonbonAleatoire(grille);

    return grille;
}

// Demande au joueur de saisir le nombre maximal de coups pour la partie
int DemanderNombreDeCoups()
{
    int nombreDeCoups;
    while (true)
    {
        Console.WriteLine("Entrez le nombre maximal de coups pour cette partie :");
        string? input = Console.ReadLine();

        // Vérifier que l'entrée n'est pas null avant d'essayer de la convertir
        if (input != null && int.TryParse(input, out nombreDeCoups))
        {
            return nombreDeCoups;
        }
        Console.WriteLine("Entrée invalide. Veuillez entrer un nombre entier.");
    }
}


// Gère les entrées de l'utilisateur pour les mouvements
bool GererMouvements(char[,] grille)
{
    Console.WriteLine("Entrez un mouvement (8: haut, 6: droite, 4: gauche, 2: bas):");
    char mouvement = Console.ReadKey().KeyChar;
    Console.ReadLine(); // Consomme le reste de la ligne

    char[,] grilleAvantMouvement = (char[,])grille.Clone(); // Copie de la grille avant le mouvement

    switch (mouvement)
    {
        case '8':
            DeplacerVersHaut(grille);
            FusionnerBonbonsHaut(grille);
            DeplacerVersHaut(grille);
            break;
        case '6':
            DeplacerVersDroite(grille);
            FusionnerBonbonsDroite(grille);
            DeplacerVersDroite(grille);
            break;
        case '4':
            DeplacerVersGauche(grille);
            FusionnerBonbonsGauche(grille);
            DeplacerVersGauche(grille);
            break;
        case '2':
            DeplacerVersBas(grille);
            FusionnerBonbonsBas(grille);
            DeplacerVersBas(grille);
            break;
        default:
            Console.WriteLine("\nEntrée non valide. Réessayez.");
            return false;
    }

    // Vérifie si un changement a été fait sur la grille
    if (EstIdentique(grille, grilleAvantMouvement))
    {
        Console.WriteLine("\n ⚠  Mouvement sans effet. Réessayez.");
        return false;
    }

    AjouterBonbonAleatoire(grille); // Ajoute un bonbon après un mouvement avec effet
    return true;
}


// Méthode pour comparer deux grilles de jeu et vérifier si elles sont identiques.
bool EstIdentique(char[,] grille1, char[,] grille2)
{
    // Parcourir chaque ligne de la grille
    for (int i = 0; i < grille1.GetLength(0); i++)
    {
        // Parcourir chaque colonne de la grille
        for (int j = 0; j < grille1.GetLength(1); j++)
        {
            // Comparer les éléments correspondants des deux grilles
            // Si un élément dans la même position diffère entre les deux grilles,
            // alors les grilles ne sont pas identiques
            if (grille1[i, j] != grille2[i, j])
                return false; // Retourne false dès qu'une différence est trouvée
        }
    }

    // Si aucune différence n'a été trouvée après avoir parcouru toute la grille,
    // cela signifie que les deux grilles sont identiques
    return true;
}


// Vérifie si la partie est terminée
bool VerifierFinDePartie(char[,] grille, int nombreDeCoups, int coupsJoues)
{
    if (coupsJoues >= nombreDeCoups) return true; // Nombre maximal de coups atteint
    return false;
}

// Affiche la grille et les informations du jeu
void AfficherGrilleEtInfos(char[,] grille, int coupsRestants, int coupsJoues)
{
    int taille = grille.GetLength(1);
    string ligneHorizontale = "+";

    for (int i = 0; i < taille; i++)
    {
        ligneHorizontale += "---+";
    }

    // Afficher la ligne du haut de la grille
    Console.WriteLine(ligneHorizontale);

    for (int i = 0; i < grille.GetLength(0); i++)
    {
        for (int j = 0; j < grille.GetLength(1); j++)
        {
            Console.Write($"| {grille[i, j]} ");
        }
        Console.WriteLine("|");

        // Afficher la ligne horizontale après chaque ligne de la grille
        Console.WriteLine(ligneHorizontale);
    }

    Console.WriteLine($"Nombre de coups joués : {coupsJoues}");
    Console.WriteLine($"Nombre de coups restants : {coupsRestants}");
    Console.WriteLine("Rappel des commandes : 8 pour haut ⬆, 6 pour droite➡, 4 pour gauche⬅, 2 pour bas ⬇.");
}


// Ajoute un bonbon aléatoire à la grille
void AjouterBonbonAleatoire(char[,] grille)
{
    Random rnd = new Random();
    int positionL, positionC;

    do
    {
        positionL = rnd.Next(grille.GetLength(0));
        positionC = rnd.Next(grille.GetLength(1));
    }
    while (grille[positionL, positionC] != ' ');

    // On utilise rnd.Next(8) pour générer un nombre entre 0 et 7
    // Si ce nombre est 0 (une chance sur 8), le bonbon sera '@'
    // Sinon (les 7 autres cas sur 8), le bonbon sera '*'
    char bonbon = rnd.Next(8) == 0 ? '@' : '*';
    grille[positionL, positionC] = bonbon;
}


// Déplace les bonbons vers le haut
void DeplacerVersHaut(char[,] grille)
{
    for (int col = 0; col < grille.GetLength(1); col++)
    {
        int positionInsertion = 0;
        for (int row = 0; row < grille.GetLength(0); row++)
        {
            if (grille[row, col] != ' ')
            {
                grille[positionInsertion, col] = grille[row, col];
                if (row != positionInsertion)
                {
                    grille[row, col] = ' ';
                }
                positionInsertion++;
            }
        }
    }
}

// Déplace les bonbons vers le bas
void DeplacerVersBas(char[,] grille)
{
    // On parcourt chaque colonne de la grille
    for (int col = 0; col < grille.GetLength(1); col++)
    {
        // On initialise la position d'insertion au bas de la colonne
        int positionInsertion = grille.GetLength(0) - 1;

        // On parcourt la colonne de bas en haut
        for (int row = grille.GetLength(0) - 1; row >= 0; row--)
        {
            // On vérifie si la case courante contient un bonbon (n'est pas vide)
            if (grille[row, col] != ' ')
            {
                // On déplace le bonbon à la position d'insertion actuelle
                grille[positionInsertion, col] = grille[row, col];

                // Si la position d'origine est différente de la position d'insertion, on vide la position d'origine
                if (row != positionInsertion)
                {
                    grille[row, col] = ' ';
                }

                // On décrémente la position d'insertion pour le prochain bonbon à déplacer
                positionInsertion--;
            }
        }
    }
}

//Même principe pour toutes les fonctions de déplacement qui suivent :

// Déplace les bonbons vers la droite
void DeplacerVersDroite(char[,] grille)
{
    for (int row = 0; row < grille.GetLength(0); row++)
    {
        int positionInsertion = grille.GetLength(1) - 1;
        for (int col = grille.GetLength(1) - 1; col >= 0; col--)
        {
            if (grille[row, col] != ' ')
            {
                grille[row, positionInsertion] = grille[row, col];
                if (col != positionInsertion)
                {
                    grille[row, col] = ' ';
                }
                positionInsertion--;
            }
        }
    }
}

// Déplace les bonbons vers la gauche
void DeplacerVersGauche(char[,] grille)
{
    for (int row = 0; row < grille.GetLength(0); row++)
    {
        int positionInsertion = 0;
        for (int col = 0; col < grille.GetLength(1); col++)
        {
            if (grille[row, col] != ' ')
            {
                if (col != positionInsertion)
                {
                    grille[row, positionInsertion] = grille[row, col];
                    grille[row, col] = ' ';
                }
                positionInsertion++;
            }
        }
    }
}



// Fusionne les bonbons lors du déplacement vers le haut
void FusionnerBonbonsHaut(char[,] grille)
{
    // Parcourir chaque colonne de la grille
    for (int col = 0; col < grille.GetLength(1); col++)
    {
        // Parcourir chaque ligne de la colonne, sauf la dernière
        for (int row = 0; row < grille.GetLength(0) - 1; row++)
        {
            // Vérifier si le bonbon courant et le bonbon directement au-dessus sont identiques
            // et que la case courante n'est pas vide
            if (grille[row, col] == grille[row + 1, col] && grille[row, col] != ' ')
            {
                // Si les bonbons sont identiques, fusionner le bonbon courant en le remplaçant
                // par le prochain type de bonbon (géré par la méthode ProchainBonbon)
                grille[row, col] = ProchainBonbon(grille[row, col]);

                // La case du bonbon fusionné juste au-dessus est vidée
                grille[row + 1, col] = ' ';
            }
        }
    }
}

//Idem pour toutes les fonctions de fusion qui suivent :

// Fusionne les bonbons lors du déplacement vers le bas
void FusionnerBonbonsBas(char[,] grille)
{
    for (int col = 0; col < grille.GetLength(1); col++)
    {
        for (int row = grille.GetLength(0) - 1; row > 0; row--)
        {
            if (grille[row, col] == grille[row - 1, col] && grille[row, col] != ' ')
            {
                grille[row, col] = ProchainBonbon(grille[row, col]);
                grille[row - 1, col] = ' ';
            }
        }
    }
}

// Fusionne les bonbons lors du déplacement vers la gauche
void FusionnerBonbonsGauche(char[,] grille)
{
    for (int row = 0; row < grille.GetLength(0); row++)
    {
        for (int col = 0; col < grille.GetLength(1) - 1; col++)
        {
            if (grille[row, col] == grille[row, col + 1] && grille[row, col] != ' ')
            {
                grille[row, col] = ProchainBonbon(grille[row, col]);
                grille[row, col + 1] = ' ';
            }
        }
    }
}

// Fusionne les bonbons lors du déplacement vers la droite
void FusionnerBonbonsDroite(char[,] grille)
{
    for (int row = 0; row < grille.GetLength(0); row++)
    {
        for (int col = grille.GetLength(1) - 1; col > 0; col--)
        {
            if (grille[row, col] == grille[row, col - 1] && grille[row, col] != ' ')
            {
                grille[row, col] = ProchainBonbon(grille[row, col]);
                grille[row, col - 1] = ' ';
            }
        }
    }
}


// Cette méthode détermine le prochain type de bonbon après une fusion
char ProchainBonbon(char bonbon)
{
    // Utilisation d'une structure switch pour traiter les différents types de bonbons
    switch (bonbon)
    {
        case '*':
            // Si le bonbon actuel est '*', il fusionne en '@'
            return '@';

        case '@':
            // Si le bonbon actuel est '@', il fusionne en 'o'
            return 'o';

        case 'o':
            // Si le bonbon actuel est 'o', il fusionne en 'J'
            return 'J';

        default:
            // Pour 'J' ou tout autre cas inattendu, la fusion ne produit aucun nouveau bonbon
            // (Peut servir pour gérer une erreur ou une condition inattendue)
            return ' ';
    }
}

//Calcule le score final
// Cette méthode calcule le score total basé sur les bonbons présents dans la grille
int CalculerScore(char[,] grille)
{
    int score = 0; // Initialise le score à 0

    // On parcourt chaque cellule de la grille
    for (int i = 0; i < grille.GetLength(0); i++) // On parcourt les lignes de la grille
    {
        for (int j = 0; j < grille.GetLength(1); j++) // On parcourt les colonnes de la grille
        {
            // Selon le type de bonbon dans la cellule, on ajoute des points au score
            switch (grille[i, j])
            {
                case '*':
                    // Si le bonbon est de type '*', on ajoute 1 point
                    score += 1;
                    break;
                case '@':
                    // Si le bonbon est de type '@', on ajoute 3 points
                    score += 3;
                    break;
                case 'o':
                    // Si le bonbon est de type 'o', on ajoute 7 points
                    score += 7;
                    break;
                case 'J':
                    // Si le bonbon est de type 'J', on ajoute 15 points
                    score += 15;
                    break;
            }
        }
    }

    // On retourne le score total après avoir parcouru toute la grille
    return score;
}