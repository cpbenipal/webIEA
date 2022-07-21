export default class CustomPalette {
  constructor(create, elementFactory, palette, translate,eventBus) {
    this.create = create;
    this.elementFactory = elementFactory;
    this.translate = translate;
    this.eventBus=eventBus;

    palette.registerProvider(this);
  }

  getPaletteEntries(element) {
    const {
      create,
      elementFactory,
      translate,
      eventBus
    } = this;

    
    // function searchButton(event, element) {
    //   eventBus.fire('button.search');
    // }
    // function newButton(event, element) {
    //   eventBus.fire('button.new');
    // }
    // function openButton(event, element) {
    //   eventBus.fire('button.open');
    // }
    // function saveButton(event, element) {
    //   eventBus.fire('button.save');
    // }
    // function searchArrange(event, element) {
    //   eventBus.fire('button.arrange');
    // }
    return {
      
    //   'button.search': {
    //     group: 'buttons',
    //     className: 'bpmn-icon-button-search',
    //     title: translate('Search'),
    //     text:translate('Search'),
    //     draggable:false,
    //     action: {
    //       click: searchButton
    //     }
    //   },
    //   'button.new': {
    //     group: 'buttons',
    //     className: 'bpmn-icon-button-new',
    //     title: translate('New'),
    //     text:translate('New'),
    //     draggable:false,
    //     action: {
    //       click: newButton
    //     }
    //   },
    //   'button.open': {
    //     group: 'buttons',
    //     className: 'bpmn-icon-button-open',
    //     title: translate('Open'),
    //     text:translate('Open'),
    //     draggable:false,
    //     action: {
    //       click: openButton
    //     }
    //   },
    //   'button.save': {
    //     group: 'buttons',
    //     className: 'bpmn-icon-button-save',
    //     title: translate('Save'),
    //     text:translate('Save'),
    //     draggable:false,
    //     action: {
    //       click: saveButton
    //     }
    //   },
    //   'button.arrange': {
    //     group: 'buttons',
    //     className: 'bpmn-icon-button-arrange',
    //     title: translate('Arrange'),
    //     text:translate('Arrange'),
    //     draggable:false,
    //     action: {
    //       click: searchArrange
    //     }
    //   }
     }
  }
}

CustomPalette.$inject = [
  'create',
  'elementFactory',
  'palette',
  'translate',
  'eventBus'
];