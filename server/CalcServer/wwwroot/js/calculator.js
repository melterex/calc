document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('expression');
    const btn = document.getElementById('calc-btn');
    const resultDiv = document.getElementById('result');
    const historyList = document.getElementById('history-list');

    let history = [
    ];

    async function calculate() {
        const expression = input.value.trim();
        if (!expression) return;

        btn.disabled = true;
        btn.textContent = '...';
        resultDiv.className = 'result';
        resultDiv.textContent = '';

        let data = await postCalculate(expression);

        if (data.error) {
            resultDiv.className = 'result error';
            resultDiv.textContent = data.error;
        } else {
            resultDiv.className = 'result success';
            resultDiv.textContent = '= ' + data.result;
            await renderHistory();
        }

        btn.disabled = false;
        btn.textContent = 'Вычислить';
    }

    async function renderHistory() {
        historyList.innerHTML = '';

        history = await getHistory();

        if (history.error){
            return;
        }

        history.slice().reverse().forEach(item => {
            const li = document.createElement('li');

            const exprSpan = document.createElement('span');
            exprSpan.className = 'expr';
            exprSpan.textContent = item.expression;
            exprSpan.title = item.expression;

            const resSpan = document.createElement('span');
            resSpan.className = 'res';
            resSpan.textContent = '= ' + item.result;

            li.appendChild(exprSpan);
            li.appendChild(resSpan);

            li.addEventListener('click', () => {
                input.value = item.expression;
                input.focus();
                input.setSelectionRange(0, 0);
            });

            historyList.appendChild(li);
        });
    }

    btn.onclick = calculate;

    input.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') calculate();
    });

    renderHistory();
});