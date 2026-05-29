---
name: Urban Safety Adventure
colors:
  surface: '#f3faff'
  surface-dim: '#c7dde9'
  surface-bright: '#f3faff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#e6f6ff'
  surface-container: '#dbf1fe'
  surface-container-high: '#d5ecf8'
  surface-container-highest: '#cfe6f2'
  on-surface: '#071e27'
  on-surface-variant: '#4d4632'
  inverse-surface: '#1e333c'
  inverse-on-surface: '#dff4ff'
  outline: '#7f775f'
  outline-variant: '#d0c6ab'
  surface-tint: '#705d00'
  primary: '#705d00'
  on-primary: '#ffffff'
  primary-container: '#ffd600'
  on-primary-container: '#705d00'
  inverse-primary: '#e9c400'
  secondary: '#006e1c'
  on-secondary: '#ffffff'
  secondary-container: '#91f78e'
  on-secondary-container: '#00731e'
  tertiary: '#0061a4'
  on-tertiary: '#ffffff'
  tertiary-container: '#c0dbff'
  on-tertiary-container: '#0061a4'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffe170'
  primary-fixed-dim: '#e9c400'
  on-primary-fixed: '#221b00'
  on-primary-fixed-variant: '#544600'
  secondary-fixed: '#94f990'
  secondary-fixed-dim: '#78dc77'
  on-secondary-fixed: '#002204'
  on-secondary-fixed-variant: '#005313'
  tertiary-fixed: '#d1e4ff'
  tertiary-fixed-dim: '#9ecaff'
  on-tertiary-fixed: '#001d36'
  on-tertiary-fixed-variant: '#00497d'
  background: '#f3faff'
  on-background: '#071e27'
  surface-variant: '#cfe6f2'
typography:
  display-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '800'
    lineHeight: 40px
  headline-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  body-lg:
    fontFamily: Quicksand
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-md:
    fontFamily: Quicksand
    fontSize: 16px
    fontWeight: '500'
    lineHeight: 24px
  label-lg:
    fontFamily: Quicksand
    fontSize: 14px
    fontWeight: '700'
    lineHeight: 20px
    letterSpacing: 0.05em
  headline-lg-mobile:
    fontFamily: Plus Jakarta Sans
    fontSize: 28px
    fontWeight: '800'
    lineHeight: 36px
rounded:
  sm: 0.5rem
  DEFAULT: 1rem
  md: 1.5rem
  lg: 2rem
  xl: 3rem
  full: 9999px
spacing:
  base: 8px
  xs: 4px
  sm: 12px
  md: 24px
  lg: 40px
  xl: 64px
  touch-target-min: 48px
---

## Brand & Style

The design system is built to be an inviting, high-energy educational environment tailored for children aged 8-14. It draws heavy inspiration from contemporary 3D mobile titles like *Toca Boca* and *PK XD*, prioritizing a "tactile-digital" feel where every interactive element looks like a physical, squishy button or a polished plastic toy.

The visual style is **Cartoon-Tactile**. It avoids the flatness of traditional corporate UI in favor of depth, using soft inner glows, subtle gradients, and thick "clay-morphic" shadows. The atmosphere evokes a sunny day in Addis Ababa—warm, optimistic, and bustling with life, yet structured enough to guide a child through safety lessons without distraction.

**Key Principles:**
- **Safety First:** Red, yellow, and green are used functionally to mirror traffic signals.
- **Physicality:** Every button should feel like it can be physically pressed down.
- **Clarity:** Large, legible typography and high-contrast iconography ensure the educational content is never lost in the playfulness.

## Colors

The palette is anchored in "Safe-Vibrant" tones. We use a high-saturation **Sunshine Yellow** as the primary brand color to reflect the energy of Addis Ababa. **Lush Green** and **Sky Blue** serve as secondary and tertiary anchors, providing a balanced, natural feel.

- **Primary (Yellow):** Used for main actions, hero buttons, and high-priority rewards (stars).
- **Secondary (Green):** Specifically reserved for "Go" actions, safety confirmations, and success states.
- **Tertiary (Blue):** Used for educational panels, navigation, and background elements.
- **Functional Colors:** Red is strictly used for "Stop" or "Danger" scenarios. A deep slate neutral is used for text to maintain high legibility against bright backgrounds without the harshness of pure black.

## Typography

Typography is chosen for its friendliness and accessibility. **Plus Jakarta Sans** provides a modern, geometric punch for headlines, while **Quicksand** offers a soft, rounded aesthetic for body text that is highly readable for developing readers.

- **Headlines:** Use Bold or ExtraBold weights to create a "sticker" effect. Headlines often benefit from a thin white text-shadow or stroke to pop against busy game backgrounds.
- **Body Text:** Always use medium to semi-bold weights. Avoid light weights as they disappear on mobile screens under various lighting conditions.
- **Line Heights:** Generous line heights are maintained to prevent text from feeling cramped, ensuring a breezy, educational reading experience.

## Layout & Spacing

The layout philosophy follows a **Dynamic Grid** model that prioritizes large, "thumb-friendly" interaction zones. Since this is a mobile game UI, we use safe-area margins to account for device notches and rounded screen corners.

- **Interaction Zones:** All primary buttons are placed within the "Natural Thumb Zone" (bottom two-thirds of the screen). 
- **Rhythm:** An 8px base unit drives the system. Padding inside cards is typically `md` (24px), while gutters between small items like star ratings are `sm` (12px).
- **Negative Space:** Whitespace is intentionally used to separate "Game World" elements from "UI Overlay" elements, ensuring the player knows exactly what is a clickable menu versus a background building.

## Elevation & Depth

This design system uses **Tactile Layering** to create hierarchy. 

1.  **The Base Layer:** The game world or a soft tinted background.
2.  **The Panel Layer:** Large white or pale blue containers with "Soft Shadows" (15% opacity, 20px blur, 8px offset) to make them look like they are floating just above the screen.
3.  **The Interactive Layer:** Buttons use "Hard-Soft Shadows." A darker, more saturated version of the button's color is used as a 4px bottom-border to simulate 3D thickness.
4.  **The Feedback Layer:** When pressed, buttons shift down by 2px and their bottom-shadow disappears, providing immediate haptic-visual confirmation.

Avoid traditional "floating" neomorphism; instead, use inner glows on buttons to make them look "inflated" and friendly.

## Shapes

The shape language is **Ultra-Rounded**. There are no sharp corners in the design system.

- **Standard Elements:** Use `rounded-lg` (1rem / 16px) for cards and modals.
- **Buttons & Chips:** Always use `pill-shaped` (fully rounded corners) to maximize the "toy" aesthetic and ensure safety is visually communicated through softness.
- **Icons:** Icons should be housed within circular or "squircle" containers.
- **Stroke Widths:** Use thick, consistent strokes (3px - 5px) for borders on UI elements to give them a "sticker" or "cartoon" outline feel.

## Components

### Buttons
- **Hero Button:** Large, yellow, pill-shaped with a 4px dark-yellow bottom shadow. White text with a thin dark outline.
- **Secondary Button:** Green or Blue with a 2px offset. Used for "Back" or "Settings."
- **Icon Buttons:** Circular with a white border.

### Cards & Modals
- Content is housed in white, high-roundedness containers. 
- Modals feature a "header bar" that slightly overlaps the top of the container, often in a contrasting color (e.g., a blue header on a white modal).

### Input Fields
- Soft, light-blue recessed wells. Instead of a flat line, use a fully rounded container with a subtle inner shadow to make it look "sunken" into the card.

### Progress & Status
- **Traffic Lights:** A vertical pill containing three circles. The active light has a high-intensity "glow" (outer shadow of the same color).
- **Star Ratings:** Chunky, 5-point stars. Earned stars are vibrant yellow with a gold stroke; unearned stars are a translucent gray-blue.

### Lists
- Each list item is a "mini-card" with a horizontal layout. Include a large icon on the left and a chevron or "Go" button on the right. Items should have generous vertical spacing (12px+) to prevent accidental taps.