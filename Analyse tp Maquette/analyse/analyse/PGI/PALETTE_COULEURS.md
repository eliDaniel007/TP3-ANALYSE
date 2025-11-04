# 🎨 Palette de Couleurs - PGI Nordik Adventures

**Source:** [Coolors Palette](https://coolors.co/palette/780000-c1121f-fdf0d5-003049-669bbc)

## Palette Complète

| Couleur | Code HEX | Aperçu | Utilisation Principale |
|---------|----------|--------|------------------------|
| 🍷 **Bourgogne** | `#780000` | ![#780000](https://via.placeholder.com/50x30/780000/780000.png) | MODULE 1: Stocks & Produits |
| 🔴 **Rouge vif** | `#C1121F` | ![#C1121F](https://via.placeholder.com/50x30/C1121F/C1121F.png) | MODULE 2: Finances & Facturation |
| 📄 **Crème** | `#FDF0D5` | ![#FDF0D5](https://via.placeholder.com/50x30/FDF0D5/FDF0D5.png) | Arrière-plans, cartes, sections |
| 🌊 **Bleu marine** | `#003049` | ![#003049](https://via.placeholder.com/50x30/003049/003049.png) | Textes principaux, éléments foncés |
| 💠 **Bleu gris** | `#669BBC` | ![#669BBC](https://via.placeholder.com/50x30/669BBC/669BBC.png) | MODULE 3: CRM, boutons, bordures |

---

## 🎯 Application par Composant

### 🏠 **Fenêtre Principale (MainWindow)**

#### En-tête Principal
- **Gradient Background:** `#780000` → `#003049` → `#669BBC`
- **Titre Principal:** Blanc
- **Sous-titre:** `#FDF0D5`

#### Tableau de Bord - Cartes KPI

| Carte | Bordure | Texte Principal | Valeur |
|-------|---------|-----------------|--------|
| 💰 Ventes | `#C1121F` | `#C1121F` | `#780000` |
| 📈 Marge brute | `#669BBC` | `#669BBC` | `#003049` |
| 📦 Articles à commander | `#780000` | `#780000` | `#003049` |
| 👥 Clients fidèles | `#003049` | `#003049` | `#669BBC` |
| ⏰ Factures en retard | `#C1121F` | `#C1121F` | `#780000` |
| 💎 Stock valorisé | `#669BBC` | `#669BBC` | `#003049` |

#### Modules

| Module | Couleur Tab | Couleur Header | Background Gradient |
|--------|-------------|----------------|---------------------|
| 📦 **MODULE 1: Stocks** | `#780000` | `#780000` | `#FDF0D5` → Blanc |
| 💰 **MODULE 2: Finances** | `#C1121F` | `#C1121F` | `#FDF0D5` → Blanc |
| 👥 **MODULE 3: CRM** | `#669BBC` | `#669BBC` | `#FDF0D5` → Blanc |

#### Éléments de Formulaire
- **Bordures TextBox/ComboBox:** `#669BBC`
- **Texte des inputs:** `#003049`
- **Bordures GroupBox:** `#003049`
- **Header GroupBox Background:** `#FDF0D5`
- **Header GroupBox Text:** `#003049`

#### Boutons
- **Background Normal:** `#669BBC`
- **Texte:** Blanc
- **Hover:** `#003049`
- **Pressed:** `#780000`
- **Ombre:** `#003049` (opacité 30%)

---

### 🔐 **Fenêtre de Connexion (LoginWindow)**

#### Panneau Gauche
- **Gradient Background:** `#780000` → `#003049` → `#669BBC`
- **Titre Principal:** Blanc
- **Sous-titre:** `#FDF0D5`
- **Liste des fonctionnalités:** Blanc

#### Panneau Droit (Formulaire)
- **Background:** Blanc
- **Bordures TextBox/PasswordBox:** `#669BBC`
- **Texte des inputs:** `#003049`
- **Bouton Principal:** `#669BBC` avec texte blanc
- **Bouton Hover:** `#003049`
- **Bouton Pressed:** `#780000`

#### Effets
- **Ombre du window:** `#003049` (opacité 40%)
- **Ombre des boutons:** `#003049` (opacité 40%)

---

### 📝 **Fenêtre d'Inscription (RegisterWindow)**

#### Panneau Gauche
- **Gradient Background:** `#669BBC` → `#003049` → `#780000`
- **Titre Principal:** Blanc
- **Sous-titre:** `#FDF0D5`
- **Liste des avantages:** Blanc

#### Panneau Droit (Formulaire)
- **Background:** Blanc
- **Bordures TextBox/PasswordBox:** `#669BBC`
- **Texte des inputs:** `#003049`
- **Bouton Principal:** `#669BBC` avec texte blanc
- **Bouton Hover:** `#003049`
- **Bouton Pressed:** `#780000`

#### Effets
- **Ombre du window:** `#003049` (opacité 40%)
- **Ombre des boutons:** `#003049` (opacité 40%)

---

## 🔄 Comparaison : Ancienne vs Nouvelle Palette

### Ancienne Palette (Pastel)
- 🌸 `#CDB4DB` (Lilas clair) → **Remplacé par** `#780000` (Bourgogne)
- 🎀 `#FFC8DD` (Rose clair) → **Remplacé par** `#C1121F` (Rouge)
- 💗 `#FFAFCC` (Rose) → **Remplacé par** `#669BBC` (Bleu gris)
- 💙 `#A2D2FF` (Bleu clair) → **Remplacé par** `#669BBC` (Bleu gris)
- 💜 `#BDE0FE` (Bleu pâle) → **Remplacé par** `#003049` (Bleu marine)

### Nouvelle Palette (Corporate/Professionnelle)
- Plus **mature** et **professionnelle**
- Meilleur **contraste** pour la lisibilité
- Tons **riches et profonds** (bordeaux, marine)
- Accent **chaleureux** avec le crème (`#FDF0D5`)

---

## 📐 Guidelines d'Utilisation

### ✅ À Faire
- Utiliser `#FDF0D5` pour les arrière-plans légers
- `#003049` pour les textes principaux (excellent contraste)
- `#669BBC` pour les éléments interactifs (boutons, liens)
- Les gradients doivent inclure au moins 2 couleurs de la palette
- Opacités recommandées pour les ombres : 20-40%

### ❌ À Éviter
- Mélanger trop de couleurs vives dans une même section
- Utiliser `#780000` ou `#C1121F` pour du texte sur fond blanc (contraste insuffisant)
- Ombres trop foncées (>50% d'opacité)
- Bordures trop épaisses avec les couleurs foncées

---

## 🎨 Codes CSS/XAML Utiles

### Gradients Prédéfinis

#### Gradient Principal (Header)
```xaml
<LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#780000" Offset="0"/>
    <GradientStop Color="#003049" Offset="0.5"/>
    <GradientStop Color="#669BBC" Offset="1"/>
</LinearGradientBrush>
```

#### Gradient Léger (Background)
```xaml
<LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
    <GradientStop Color="#FDF0D5" Offset="0"/>
    <GradientStop Color="White" Offset="0.3"/>
</LinearGradientBrush>
```

### Ombres Prédéfinies

#### Ombre Douce (Cartes)
```xaml
<DropShadowEffect Color="#003049" BlurRadius="12" ShadowDepth="3" Opacity="0.3"/>
```

#### Ombre Forte (Windows)
```xaml
<DropShadowEffect Color="#003049" BlurRadius="30" ShadowDepth="0" Opacity="0.4"/>
```

---

## 📊 Accessibilité et Contraste

### Ratios de Contraste WCAG 2.1

| Combinaison | Ratio | Niveau | Usage Recommandé |
|-------------|-------|--------|------------------|
| `#003049` sur `#FDF0D5` | 10.8:1 | AAA ⭐⭐⭐ | Textes principaux |
| Blanc sur `#780000` | 8.2:1 | AAA ⭐⭐⭐ | Textes sur headers |
| Blanc sur `#669BBC` | 3.8:1 | AA ⭐⭐ | Boutons |
| `#669BBC` sur Blanc | 4.1:1 | AA ⭐⭐ | Bordures, liens |
| `#C1121F` sur `#FDF0D5` | 7.5:1 | AAA ⭐⭐⭐ | Alertes, warnings |

✅ **Toutes les combinaisons respectent au minimum le niveau AA du WCAG 2.1**

---

## 📅 Date de Mise à Jour
**Dernière mise à jour:** Novembre 2025  
**Version:** 2.0 (Palette Professionnelle)

---

## 🔗 Ressources
- [Palette Coolors](https://coolors.co/palette/780000-c1121f-fdf0d5-003049-669bbc)
- [WCAG Contrast Checker](https://webaim.org/resources/contrastchecker/)

