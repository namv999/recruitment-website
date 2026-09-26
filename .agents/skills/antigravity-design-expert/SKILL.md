---
name: antigravity-design-expert
description: UI/UX engineering skill for building modern, minimal, responsive, and professional recruitment website interfaces using ASP.NET MVC Razor Views, Bootstrap, custom CSS, and Vanilla JavaScript with Modern Minimalism + Bento Grid design principles.
risk: safe
source: project
---

# Antigravity UI & Motion Design Expert — Recruitment Website

## When to Use

Use this skill when:
- Building or modifying UI/UX for the recruitment management website.
- Creating or redesigning Razor Views.
- Designing dashboards, recruitment listings, job details, candidate management, application management, forms, tables, navigation, and administrative interfaces.
- Improving visual consistency, responsiveness, spacing, typography, cards, buttons, forms, and interactive states.
- Adding subtle and purposeful animations or micro-interactions.
- Creating interfaces following the project's **Modern Minimalism + Bento Grid** design direction.

Do NOT use this skill to change the project's backend architecture, database structure, business logic, authentication flow, Entity Framework configuration, Web API architecture, or routing unless explicitly requested.

---

# 1. Project Technology Constraints

The project uses:
- **ASP.NET MVC**
- **Razor Views**
- **Entity Framework**
- **Web API**
- **SQL Server**
- **Bootstrap**
- **Custom CSS**
- **Vanilla JavaScript**

## Mandatory Rules

### DO
- Use Razor Views for page rendering.
- Use Bootstrap for the primary layout and responsive system.
- Write additional custom CSS when Bootstrap alone cannot achieve the required design.
- Use Vanilla JavaScript for client-side interactions.
- Reuse existing project components, layouts, styles, and scripts whenever possible.
- Follow the project's existing architecture and naming conventions.

### DO NOT
- Do NOT introduce React.
- Do NOT introduce Next.js.
- Do NOT introduce Tailwind CSS.
- Do NOT replace Razor Views with React components.
- Do NOT introduce another frontend framework without explicit approval.
- Do NOT rewrite the existing ASP.NET MVC architecture merely to achieve a visual effect.

---

# 2. Core Design Direction

The website follows:
**Modern Minimalism + Bento Grid**

The interface should feel:
- Modern
- Clean
- Professional
- Spacious
- Structured
- Lightweight
- Premium
- Easy to scan
- Appropriate for a recruitment/business platform

Avoid excessive decoration.

The visual design should prioritize:
1. Information hierarchy
2. Usability
3. Consistency
4. Readability
5. Responsive behavior
6. Subtle visual polish

Visual effects must support the interface rather than distract from its content.

---

# 3. Modern Minimalism

Use a restrained visual language.

## Principles
- Prefer whitespace over unnecessary decoration.
- Use clear typography hierarchy.
- Keep color usage controlled.
- Avoid excessive gradients.
- Avoid excessive shadows.
- Avoid excessive borders.
- Avoid unnecessary animations.
- Keep components visually simple but refined.
- Maintain consistent spacing throughout the application.

The interface should look intentional even when most elements are static.

---

# 4. Bento Grid Design

Use Bento Grid principles when the page benefits from modular information blocks.

A Bento layout may contain:

```text
┌───────────────────────┬──────────────┐
│                       │              │
│     Main Content      │   Summary    │
│                       │              │
├───────────────┬───────┴──────────────┤
│               │                      │
│   Statistic   │     Secondary Data   │
│               │                      │
└───────────────┴──────────────────────┘
```

Use Bento Grid particularly for:
- Admin dashboards
- Recruitment statistics
- Job summaries
- Candidate summaries
- Company information
- Application status
- Recruitment analytics
- Overview pages

Bento Grid is not mandatory for every page.
Do not force a Bento layout onto forms, tables, or workflows where a conventional layout provides better usability.

---

# 5. Bootstrap Usage

Bootstrap should provide the foundation for:
- Grid system
- Containers
- Rows and columns
- Responsive breakpoints
- Buttons
- Forms
- Tables
- Modals
- Alerts
- Dropdowns
- Navigation
- Utility classes

Prefer Bootstrap utilities when they are sufficient.

Use custom CSS for:
- Project-specific visual identity
- Custom cards
- Bento layouts
- Advanced spacing
- Custom typography
- Hover states
- Custom transitions
- Visual effects
- Design-system components

Do not duplicate Bootstrap functionality unnecessarily.

---

# 6. Custom CSS

Custom CSS should be:
- Modular
- Readable
- Reusable
- Scoped where appropriate
- Consistent with the project's existing CSS architecture

Avoid inline styles unless there is a strong reason.

Prefer reusable classes:
```html
<div class="job-card">
```

instead of repeatedly writing:
```html
<div style="...">
```

Avoid creating multiple classes that solve the same visual problem.
Before adding a new CSS class, check whether an existing Bootstrap utility or project class already solves the problem.

---

# 7. Typography

Typography must create a clear hierarchy.

Use:
```text
Page Title
    ↓
Section Title
    ↓
Component Title
    ↓
Body Text
    ↓
Supporting Text
```

Prioritize:
- Readability
- Appropriate line height
- Consistent font sizing
- Clear contrast
- Consistent heading hierarchy

Avoid excessive font weights and decorative typography.

---

# 8. Color System

Use a restrained palette.
Define project colors centrally when possible:

```css
:root {
    --color-primary: ...;
    --color-secondary: ...;
    --color-background: ...;
    --color-surface: ...;
    --color-text: ...;
    --color-muted: ...;
    --color-border: ...;
    --color-success: ...;
    --color-warning: ...;
    --color-danger: ...;
}
```

Do not introduce random colors for individual components.

Semantic colors should have consistent meaning:
- **Success** → successful / active / approved
- **Warning** → pending / attention
- **Danger**  → rejected / destructive action
- **Muted**   → secondary information
- **Primary** → primary interaction

---

# 9. Cards & Components

Cards should follow the Modern Minimalism direction.

Prefer:
- Moderate border radius
- Subtle border
- Soft shadow
- Clean internal spacing
- Clear hierarchy
- Consistent padding

Avoid:
- Excessive shadows
- Excessive rounded corners
- Heavy gradients
- Excessive glassmorphism
- Decorative elements without functional purpose

A card should communicate its content before communicating its decoration.

---

# 10. Buttons

Buttons must clearly communicate hierarchy.

Use Bootstrap button conventions as the foundation:
- Primary
- Secondary
- Success
- Warning
- Danger
- Outline

Primary actions should be visually stronger than secondary actions.

Examples:
- Đăng tin tuyển dụng → Primary
- Xem chi tiết → Secondary
- Chỉnh sửa → Outline
- Xóa → Danger

Do not make every button visually dominant.

---

# 11. Forms

Forms should prioritize usability.

Requirements:
- Clear labels
- Consistent spacing
- Visible focus state
- Clear validation messages
- Appropriate input sizes
- Responsive layout
- Logical grouping of fields

For long forms, divide fields into meaningful sections.
Do not hide important validation feedback inside animations.

---

# 12. Tables

Tables should remain easy to scan.

Use Bootstrap table styles as the foundation.

Recommended structure:
```text
Header
────────────────────────────
Data
Data
Data
Data
```

Use visual emphasis for:
- Status
- Important identifiers
- Dates
- Actions

Avoid excessive decoration inside tables.
For responsive tables, use Bootstrap's responsive utilities when appropriate.

---

# 13. Navigation

Navigation should remain predictable.

Use:
- Clear active states
- Consistent spacing
- Logical grouping
- Responsive behavior
- Bootstrap navigation components where appropriate

Do not introduce complex navigation animations that interfere with usability.

---

# 14. Animation & Motion

Animation should be subtle and purposeful.

Use animation for:
- Hover feedback
- Focus feedback
- Component entrance
- Modal transitions
- Loading states
- Micro-interactions
- State changes

Prefer short, smooth transitions.

Example:
```css
transition:
    transform 0.2s ease,
    box-shadow 0.2s ease,
    background-color 0.2s ease;
```

Avoid excessive motion.
Do not animate every component.
Do not use animation merely because it is technically possible.

---

# 15. Hover Effects

Hover effects should communicate interactivity.

Example:
```css
.job-card:hover {
    transform: translateY(-2px);
}
```

Keep movement subtle.

Avoid:
- Large rotations
- Aggressive scaling
- Excessive parallax
- Distracting effects

---

# 16. Responsive Design

Every UI component must work across:
- Desktop
- Tablet
- Mobile

Use Bootstrap responsive breakpoints.

Always consider:
- Navigation collapse
- Grid stacking
- Card resizing
- Table overflow
- Form layout
- Button wrapping
- Typography scaling
- Spacing reduction

Do not design desktop first and simply assume mobile will work.

---

# 17. Accessibility

Always preserve usability for users with accessibility needs.

Requirements:
- Semantic HTML where possible
- Proper labels for form controls
- Keyboard-accessible interactions
- Visible focus states
- Sufficient color contrast
- Meaningful button text
- Do not rely exclusively on color to communicate status

Respect:
```css
@media (prefers-reduced-motion: reduce)
```
When reduced motion is requested, reduce or disable non-essential animations.

---

# 18. Performance

Prioritize performance.

Avoid:
- Continuous expensive animations
- Excessive DOM manipulation
- Large unnecessary JavaScript dependencies
- Heavy visual effects without justification
- Continuous animation of expensive CSS properties

Prefer animating:
- `transform`
- `opacity`

Avoid continuously animating:
- `box-shadow`
- `filter`

unless there is a clear reason.

---

# 19. Component Reusability

Build reusable UI components and patterns.

Examples:
- Job Card
- Candidate Card
- Status Badge
- Statistic Card
- Search Bar
- Filter Panel
- Pagination
- Modal
- Alert
- Empty State
- Loading State

When the same UI pattern appears more than once, consider extracting a reusable pattern instead of duplicating markup and CSS.

---

# 20. Existing Project First

Before creating a new UI component:
- Inspect the existing Layout.
- Inspect existing Razor Views.
- Inspect existing Bootstrap usage.
- Inspect existing CSS.
- Inspect existing JavaScript.
- Reuse existing components where possible.
- Follow existing naming conventions.

Do not unnecessarily redesign unrelated pages.
Do not delete existing styles or markup unless explicitly requested or clearly identified as unused.

---

# 21. Design System Compliance

If the project contains `/docs/design/` or a design-system document such as `docs/design/DESIGN_SYSTEM.md`, treat that document as the project's visual source of truth.

Before implementing significant UI changes:
- Read the relevant design guidelines.
- Follow the defined colors.
- Follow typography rules.
- Follow spacing rules.
- Follow component rules.
- Follow responsive rules.
- Follow animation rules.

If this skill conflicts with the project's explicit design-system documentation, follow the project's design-system documentation.

---

# 22. Backend Safety

This skill is primarily for UI/UX implementation.

When modifying a UI:
- Do not change database schemas unless explicitly requested.
- Do not change Entity Framework models unless explicitly requested.
- Do not change authentication or authorization logic unless explicitly requested.
- Do not change Web API contracts unless explicitly requested.
- Do not change controller business logic unnecessarily.
- Do not change routing unnecessarily.

UI improvements must preserve existing application behavior.

---

# 23. Implementation Workflow

When asked to create or redesign a page:

**Step 1 — Inspect**
Understand:
```text
Controller
↓
ViewModel / Model
↓
Razor View
↓
Layout
↓
CSS
↓
JavaScript
```

**Step 2 — Identify Existing Patterns**
Look for reusable:
- Layout components
- Cards
- Buttons
- Forms
- Tables
- Alerts
- Navigation
- CSS utilities

**Step 3 — Design**
Apply:
```text
Modern Minimalism
        +
Bento Grid where appropriate
        +
Bootstrap
        +
Custom CSS
        +
Subtle motion
```

**Step 4 — Implement**
Keep implementation compatible with:
- ASP.NET MVC
- Razor
- Bootstrap
- Custom CSS
- Vanilla JavaScript

**Step 5 — Validate**
Check:
- Desktop
- Tablet
- Mobile
- Hover
- Focus
- Validation
- Empty states
- Loading states
- Accessibility
- Existing functionality

---

# 24. Final Design Principle

The final interface should communicate:
**Modern, minimal, professional, structured, and easy to use.**

The design should never sacrifice:
- Usability > Visual effects
- Consistency > Decoration
- Performance > Animation complexity

The goal is not to make the recruitment website look "flashy".
The goal is to make it look like a modern professional recruitment platform with a coherent visual identity.