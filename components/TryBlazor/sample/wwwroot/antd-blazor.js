//https://github.com/microsoft/monaco-editor/blob/main/src/basic-languages/razor/razor.ts
//https://microsoft.github.io/monaco-editor/playground.html?source=v0.44.0#example-extending-language-services-completion-provider-example
const antdComponents = [
    'Affix',
    'Alert',
    'Anchor',
    'AnchorLink',
    'AutoComplete',
    'AutoCompleteInput',
    'AutoCompleteOptGroup',
    'AutoCompleteOption',
    'AutoCompleteSearch',
    'Avatar',
    'AvatarGroup',
    'BackTop',
    'Badge',
    'BadgeRibbon',
    'Breadcrumb',
    'Button',
    'ButtonGroup',
    'DownloadButton',
    'Calendar',
    'Card',
    'CardAction',
    'CardGrid',
    'Carousel',
    'CarouselSlick',
    'Cascader',
    'Checkbox',
    'CheckboxGroup',
    'Collapse',
    'Panel',
    'Comment',
    'ConfigProvider',
    'Component',
    'FeedbackComponent',
    'ForeachLoop',
    'DatePicker',
    'MonthPicker',
    'QuarterPicker',
    'RangePicker',
    'WeekPicker',
    'YearPicker',
    'Descriptions',
    'DescriptionsItem',
    'Divider',
    'Drawer',
    'DrawerContainer',
    'Dropdown',
    'DropdownButton',
    'Empty',
    'Form',
    'FormItem',
    'FormProvider',
    'Col',
    'GridCol',
    'GridRow',
    'Row',
    'Icon',
    'IconFont',
    'Image',
    'ImagePreview',
    'ImagePreviewContainer',
    'ImagePreviewGroup',
    'InputNumber',
    'Input',
    'InputGroup',
    'InputPassword',
    'Search',
    'TextArea',
    'Sider',
    'AntList',
    'ListItem',
    'ListItemMeta',
    'Mentions',
    'MentionsOption',
    'Menu',
    'MenuItem',
    'MenuItemGroup',
    'MenuLink',
    'SubMenu',
    'Message',
    'MessageItem',
    'ComfirmContainer',
    'Confirm',
    'Dialog',
    'DialogWrapper',
    'Modal',
    'ModalCancelFooter',
    'ModalContainer',
    'ModalFooter',
    'ModalOkFooter',
    'Notification',
    'NotificationItem',
    'PageHeader',
    'Pagination',
    'PaginationOptions',
    'Popconfirm',
    'Popover',
    'Progress',
    'EnumRadioGroup',
    'Radio',
    'RadioGroup',
    'Rate',
    'RateItem',
    'Result',
    'Segmented',
    'SegmentedItem',
    'EnumSelect',
    'Select',
    'SelectOption',
    'SimpleSelect',
    'SimpleSelectOption',
    'Skeleton',
    'SkeletonElement',
    'Slider',
    'Space',
    'SpaceItem',
    'Spin',
    'CountDown',
    'Statistic',
    'Step',
    'Steps',
    'Switch',
    'ActionColumn',
    'Column',
    'ColumnDefinition',
    'GenerateColumns',
    'PropertyColumn',
    'Selection',
    'TableCell',
    'SimpleTableHeader',
    'Table',
    'TableHeader',
    'ReuseTabs',
    'TabPane',
    'Tabs',
    'Tag',
    'TimePicker',
    'Timeline',
    'TimelineItem',
    'Tooltip',
    'Transfer',
    'TreeSelect',
    'DirectoryTree',
    'Tree',
    'TreeIndent',
    'TreeNode',
    'TreeNodeCheckbox',
    'TreeNodeSwitcher',
    'TreeNodeTitle',
    'Link',
    'Paragraph',
    'Text',
    'Title',
    'Upload',
    'BreadcrumbItem',
    'CalendarHeader',
    'CardMeta',
    'AntContainer',
    'Template',
    'EmptyDefaultImg',
    'EmptySimpleImg',
    'Content',
    'Footer',
    'Header',
    'Layout',
    'MenuDivider',
    'PaginationPager',
    'SummaryCell',
    'SummaryRow',
    'TableRow',
    'FilterInputs',
    'LabelTemplateItem',
    'SelectContent',
    'SelectOptionGroup',
    'SelectSuffixIcon',
    'ResizeObserver',
    'CalendarPanelChooser',
    'Element',
    'Overlay',
    'OverlayTrigger',
    'DatePickerDatetimePanel',
    'DatePickerPanelChooser',
    'DatePickerTemplate',
    'FormRulesValidator',
    'SubMenuTrigger',
    'UploadButton',
    'DatePickerDatePanel',
    'DatePickerDecadePanel',
    'DatePickerFooter',
    'DatePickerHeader',
    'DatePickerInput',
    'DatePickerMonthPanel',
    'DatePickerQuarterPanel',
    'DatePickerYearPanel',
    'DropdownGroupButton',
    'TableRowWrapper'
];

const getComponentCompletion = (range) => {
    const antdComponentSuggestions = [
        ...antdComponents.map(k => {
            return {
                label: "AntDesign." + k,
                kind: monaco.languages.CompletionItemKind.Keyword,
                insertText: k + " ",
                documentation: `AntDesign component: <${k}> \n\n docs: https://antblazor.com/en-US/components/${k}`,
                range: range
            };
        })
    ];
    return antdComponentSuggestions;
}

const getRange = (model, position) => {
    var word = model.getWordUntilPosition(position);
    //console.log("word", word);
    var range = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn
    };
}

const provideCompletionItems = (model, position) => {
    //console.log("model, position", model, position);
    var textUntilPosition = model.getValueInRange({
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: position.lineNumber,
        endColumn: position.column
    });

    var match = textUntilPosition.match(/(-?\d*\.\d\w*)|([^\`\~\!\@\$\^\&\*\(\)\-\=\+\[\{\]\}\\\|\;\:\'\"\,\.\>\/\s]+)/g);
    if (!match) {
        return { suggestions: [] };
    }
    var range = getRange(model, position);
    return {
        suggestions: [
            ...getComponentCompletion(range),
            {
                label: 'namespace',
                kind: monaco.languages.CompletionItemKind.Keyword,
                detail: `C# namespace`,
                documentation: "The namespace keyword is used to declare a scope that contains a set of related objects. You can use a namespace to organize code elements and to create globally unique types.",
                insertText: 'namespace ',
                range: range
            }
        ]
    };
}

ready(() => {
    setTimeout(() => {
        monaco.languages.registerCompletionItemProvider('razor', {
            triggerCharacters: ['<'],
            provideCompletionItems: provideCompletionItems
        });
    }, 500);
});
