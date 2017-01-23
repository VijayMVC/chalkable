function RenderTemplate(modelStr, template) {
    try {
        var model = JSON.parse(modelStr);
        return jade.render(template, { model: model.data });
    } catch (e) {
        return '<h1>Error rendering </h1><pre>' + e.message + '\n\n' + e.stack + '</pre>';
    }
}