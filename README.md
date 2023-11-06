# What's this?

The goal of this library is to provide developers with components other than the official Antd React component.

- Each component can be referenced as a separate nuget package
- All components can have a consistent theme style


## Run it locally

### premise
- . Net 6 or later
- nodejs
- pnpm

### build and see the demo

1. `pnpm install`
    run it in the root directory of the git repository.

2. `pnpm run build`
    run it in the root directory of the git repository. This will generate CSS and JS files for each component, with the output directory being `src/{component}/wwwroot/dist`

3. `cd samples\{Component} && dotnet run`
    this will run the demo.


## Development

### Basic Guidelines

- Clone the git repository

- Create a component
    All components are located in the src directory. There **must be** an index.scss and index.ts(must have an export) file in the component directory.

- Create a sample
    For debugging, testing, or other purposes, you can create a sample project under the samples folder.

- Run the sample - Generate CSS and JS
    Run `pnpm run build` in the root directory of the git repository. This will generate CSS and JS files for each component, with the output directory being `src/{component}/wwwroot/dist`. And This will also copy all the source files used by the component to the `src/{component}/wwwroot/src/ts` directory.

- Run the sample - Run CSharp project

### Using common(basic) TS modules
The general TypeScript method exists in the `src/_scripts` directory. You can import it similar to the following to introduce in index.ts:


```ts 
// src/MonacoEditor/index.ts
import { onReady } from "../../scripts/util";
```

when running `pnpm run build`, the entry file index.ts(`src/{component}/wwwroot/src/index.ts`) maybe like as following:

```ts
import "./scripts/main"; 
import * as common from "./scripts/common";

import * as {ComponentName} from './ts/{component}';


(window as any).AntDesign.ext.common = common;
(window as any).AntDesign.ext.{ComponentName} = {ComponentName};
```


### Using common(basic) SCSS modules
The general SCSS method exists in the `src/_styles` directory. `theme-default.scss` defines the CSS variables for the default theme, and `variables.scss` defines the SCSS variables.

when running `pnpm run build`, the default theme scss file(`src/{component}/wwwroot/src/default.scss`) maybe like as following:


```scss
@import './styles/theme-default.scss';
@import './styles/variables.scss';

@import './scss/{component}/index.scss';
```

### Build a bundle asset file for all components

`pnpm run build-all` will will generate CSS and JS files for all components, with the output directory being `dist` which is in the git repository. This will also copy all the source files used by the component to the `src/bundle/src` directory.