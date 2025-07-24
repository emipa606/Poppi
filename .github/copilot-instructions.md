# Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose
This mod introduces new features and mechanics centered around unique combat abilities and interactions in RimWorld. The primary feature is the introduction of a 'Leaper' component that allows certain creatures or pawns to perform leap attacks, adding a dynamic element to combat strategies. It also includes enhancements and custom interactions to enrich the game experience.

## Key Features and Systems
- **Leap Attack Mechanic**: Implements a 'Leaper' component allowing units to launch themselves at enemies, providing a tactical advantage and enhancing gameplay dynamics.
- **Milkable Component Patch**: Adjustments to the existing milkable components to integrate with the leap mechanic.
- **Custom Damage Definitions**: New damage types are introduced to support custom interactions and effects.
- **Death Worker Customization**: Extends death actions for specific units, enriching the gameplay narrative.
- **Custom Animation and Effects**: Uses the `Poppi_FlyingObject_Leap` class and `Poppi_MoteMaker` to create visual effects and animations for the new mechanics.

## Coding Patterns and Conventions
- **Naming Conventions**: Classes follow PascalCase, methods also use PascalCase, and variables generally use camelCase. This is standard practice in C# to maintain readability and consistency.
- **Accessibility Modifiers**: Public, private, and internal access levels are used appropriately to encapsulate and protect the code.
- **Use of Static Classes**: The `DamageDefOf` and `Poppi_MoteMaker` classes are defined as static, indicating utility-focused functions that do not require instantiation.

## XML Integration
- The mod is integrated with RimWorld's XML configurations to define and modify abilities and effects. XML files should align closely with the C# logic to ensure smooth interaction between front-end definitions and back-end implementations.

## Harmony Patching
- **Harmony Patching**: The mod employs Harmony for non-intrusive modifications to existing RimWorld game functions. This methodology is crucial to maintain compatibility with other mods and updates.
- **HarmonyPatches Class**: Contains implementations for Harmony patches to adjust game runtime behavior without altering the original game code.

## Suggestions for Copilot
- **Auto-Complete Patterns**: Utilize Copilot's ability to predict and auto-complete tactical methods in new components by learning from existing code patterns like those in `CompLeaper`.
- **Refactor Suggestions**: Use Copilot to identify code refactoring opportunities for methods like `Launch` in `Poppi_FlyingObject_Leap` to reduce redundancy.
- **XML Code Integration**: Employ Copilot to suggest snippets for XML extentions aligning with C# class definitions, ensuring consistency.
- **Debugging Assistance**: Leverage Copilot to suggest debugging approaches or log insertions for methods like `ImpactSomething` to trace and resolve issues effectively.

By following these instructions and utilizing GitHub Copilot effectively, developers will be better equipped to develop and maintain this mod with advanced combat abilities and engaging interactions in RimWorld.
