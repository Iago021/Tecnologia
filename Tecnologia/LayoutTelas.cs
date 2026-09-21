using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tecnologia
{
    // Reutiliza os controles do Designer, seus valores e eventos; só reorganiza a apresentação.
    internal static class LayoutTelas
    {
        private sealed class Registro { public Registro() { } public string Estado; }
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Form, Registro> registros =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Form, Registro>();
        public static void MarcarSalvo(Form janela) { registros.GetOrCreateValue(janela).Estado = Estado(janela); }
        public static bool TemAlteracoes(Form janela)
        {
            Registro registro;
            return registros.TryGetValue(janela, out registro) && registro.Estado != Estado(janela);
        }
        public static string Estado(Control raiz)
        {
            System.Text.StringBuilder estado = new System.Text.StringBuilder();
            foreach (Control c in raiz.Controls)
            {
                if (c.Name == "txtBusca" || c.Name == "cmbCampo" || (c.Name == "cmbStatus" && c.FindForm() is OrdensForm)) continue;
                if (c is TextBox && !((TextBox)c).ReadOnly) estado.Append(c.Name).Append(':').Append(c.Text.Length).Append(':').Append(c.Text).Append(';');
                else if (c is ComboBox) estado.Append(c.Name).Append(':').Append(((ComboBox)c).SelectedIndex).Append(';');
                else if (c is CheckBox) estado.Append(c.Name).Append(':').Append(((CheckBox)c).Checked).Append(';');
                else if (c is NumericUpDown) estado.Append(c.Name).Append(':').Append(((NumericUpDown)c).Value).Append(';');
                else if (c is DateTimePicker)
                {
                    DateTimePicker data = (DateTimePicker)c;
                    estado.Append(c.Name).Append(':').Append(data.Checked ? data.Value.Date.ToString("yyyy-MM-dd") : "vazio").Append(';');
                }
                if (c is Panel || c is TabControl || c is TabPage || c is GroupBox) estado.Append(Estado(c));
            }
            return estado.ToString();
        }

        public static void Aplicar(Form janela)
        {
            janela.SuspendLayout();
            Dictionary<string, Control> controles = janela.Controls.Cast<Control>().ToDictionary(c => c.Name);
            Tema.Aplicar(janela);
            janela.Controls.Clear();
            janela.AutoScroll = false;
            janela.AutoScrollMinSize = Size.Empty;
            janela.ClientSize = new Size(1060, 720);
            janela.MinimumSize = new Size(780, 520);
            TableLayoutPanel estrutura = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Padding = new Padding(22), BackColor = Color.Transparent };
            estrutura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            janela.Controls.Add(estrutura);
            Panel cabecalho = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            estrutura.Controls.Add(cabecalho, 0, 0);
            Control titulo;
            if (!controles.TryGetValue("lblTitulo", out titulo)) titulo = new Label { Text = janela.Text };
            titulo.SetBounds(0, 10, 680, 50);
            titulo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            titulo.ForeColor = Tema.VerdeEscuro;
            titulo.BackColor = Color.Transparent;
            titulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cabecalho.Controls.Add(titulo);
            PictureBox marca = Tema.Imagem("tecnologia-logo.png");
            marca.Name = "marcaSecundaria";
            marca.Size = new Size(220, 48);
            marca.Location = new Point(760, 8);
            marca.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cabecalho.Controls.Add(marca);
            cabecalho.Resize += delegate { titulo.Width = Math.Max(300, cabecalho.Width - 250); marca.Left = cabecalho.Width - 220; };
            string nome = janela.GetType().Name;
            if (nome == "ClientesForm" || nome == "UsuariosForm" || nome == "AparelhosForm" || nome == "PecasForm")
                MontarCadastro(estrutura, controles, nome);
            else if (nome == "OrdensForm" || nome == "HistoricoForm") MontarConsulta(estrutura, controles, nome);
            else if (nome == "OrdemForm") MontarOrdem(estrutura, controles);
            else MontarFormulario(estrutura, controles, nome);
            // Todos os controles originais precisam permanecer em alguma seção.
            foreach (Control controle in controles.Values)
                if (controle.Parent == null) throw new InvalidOperationException("Controle sem seção: " + nome + "/" + controle.Name);
            janela.ResumeLayout(true);
            janela.Load += delegate { if (!janela.IsDisposed) MarcarSalvo(janela); };
            janela.FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (TemAlteracoes(janela) && !Tela.Confirmar("Sair sem salvar as alterações desta tela?")) e.Cancel = true;
            };
        }

        private static TabControl Abas(TableLayoutPanel estrutura, params string[] nomes)
        {
            TabControl abas = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Padding = new Point(20, 9) };
            foreach (string nome in nomes) abas.TabPages.Add(new TabPage(nome) { BackColor = Tema.Cartao, Padding = new Padding(16) });
            estrutura.Controls.Add(abas, 0, 1);
            return abas;
        }

        private static TableLayoutPanel Lista(TabPage pagina)
        {
            TableLayoutPanel lista = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            lista.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            lista.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            lista.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lista.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pagina.Controls.Add(lista);
            return lista;
        }

        private static FlowLayoutPanel Barra(params Control[] controles)
        {
            FlowLayoutPanel barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 5, 0, 10) };
            foreach (Control c in controles)
            {
                c.Dock = DockStyle.None; c.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                c.Margin = new Padding(0, 3, 10, 5);
                c.TabIndex = barra.Controls.Count;
                if (c is Button) c.Size = new Size(Math.Max(144, Math.Min(210, TextRenderer.MeasureText(c.Text, c.Font).Width + 32)), 38);
                else if (c is ComboBox) c.Width = 180;
                else if (c is TextBox) c.Width = 250;
                else if (c is Label) { ((Label)c).AutoSize = true; ((Label)c).MaximumSize = new Size(580, 0); }
                barra.Controls.Add(c);
            }
            return barra;
        }

        private static FlowLayoutPanel Coluna(Control pagina)
        {
            Panel rolagem = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Tema.Cartao };
            pagina.Controls.Add(rolagem);
            FlowLayoutPanel coluna = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(10) };
            rolagem.Controls.Add(coluna);
            Action ajustar = delegate
            {
                int largura = Math.Max(360, rolagem.ClientSize.Width - 28);
                coluna.MinimumSize = new Size(largura, 0); coluna.MaximumSize = new Size(largura, 0);
                foreach (Control item in coluna.Controls)
                {
                    item.Width = largura - 26;
                    FlowLayoutPanel campos = item as FlowLayoutPanel;
                    if (campos == null) continue;
                    campos.MinimumSize = new Size(largura - 26, 0);
                    campos.MaximumSize = new Size(largura - 26, 0);
                    if (!object.Equals(campos.Tag, "campos")) continue;
                    foreach (Control campo in campos.Controls)
                        campo.Width = Math.Max(300, (campos.Width - 24) / (campos.Width >= 680 ? 2 : 1));
                }
            };
            rolagem.Resize += delegate { ajustar(); };
            coluna.ControlAdded += delegate { ajustar(); };
            ajustar();
            return coluna;
        }

        private static void Secao(FlowLayoutPanel coluna, string titulo, Dictionary<string, Control> mapa, params string[] campos)
        {
            Label rotulo = new Label { Text = titulo, Height = 32, ForeColor = Tema.VerdeEscuro,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), Margin = new Padding(3, 14, 3, 4) };
            coluna.Controls.Add(rotulo);
            FlowLayoutPanel grupo = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true, Tag = "campos", Margin = new Padding(0, 0, 0, 10) };
            foreach (string nome in campos)
            {
                string[] partes = nome.Split('|');
                Control entrada = mapa[partes[0]];
                Control legenda;
                if (partes.Length > 1 && mapa.ContainsKey(partes[1])) legenda = mapa[partes[1]];
                else legenda = new Label { Text = partes.Length > 1 ? partes[1] : entrada.AccessibleName ?? entrada.Name };
                Panel par = new Panel { Width = 420, Height = entrada is TextBox && ((TextBox)entrada).Multiline ? 146 : 84,
                    Margin = new Padding(3, 3, 9, 9), TabIndex = grupo.Controls.Count };
                legenda.SetBounds(0, 0, 400, 25);
                legenda.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                legenda.Font = new Font("Segoe UI", 9);
                legenda.ForeColor = Tema.Texto;
                par.Controls.Add(legenda);
                entrada.Dock = DockStyle.None;
                entrada.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                entrada.TabIndex = 0;
                TextBox caixa = entrada as TextBox;
                if (caixa != null && !caixa.Multiline)
                {
                    Panel fundo = Tema.EnvolverCampo(caixa, 400);
                    fundo.SetBounds(0, 28, 400, 42);
                    fundo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    par.Controls.Add(fundo);
                }
                else
                {
                    entrada.SetBounds(0, 32, 400, par.Height - 40);
                    par.Controls.Add(entrada);
                }
                entrada.AccessibleName = legenda.Text;
                grupo.Controls.Add(par);
            }
            coluna.Controls.Add(grupo);
        }

        private static Control[] Escolher(Dictionary<string, Control> mapa, params string[] nomes)
        { return nomes.Where(mapa.ContainsKey).Select(n => mapa[n]).ToArray(); }

        private static void MontarCadastro(TableLayoutPanel estrutura, Dictionary<string, Control> m, string nome)
        {
            TabControl abas = Abas(estrutura, "Consultar", "Cadastrar / editar");
            TableLayoutPanel lista = Lista(abas.TabPages[0]);
            m["txtBusca"].AccessibleName = "Texto para pesquisar";
            m["cmbCampo"].AccessibleName = "Campo da pesquisa";
            lista.Controls.Add(Barra(Escolher(m, "cmbCampo", "txtBusca", "btnPesquisar", "btnNovo")), 0, 0);
            m["grade"].Dock = DockStyle.Fill; lista.Controls.Add(m["grade"], 0, 1);
            lista.Controls.Add(Barra(Escolher(m, "btnEditar", "btnExcluir", "btnMovimentar", "btnHistorico", "btnExportar")), 0, 2);
            FlowLayoutPanel editor = Coluna(abas.TabPages[1]);
            Control estado = m["lblEdicao"];
            estado.Height = 28; estado.ForeColor = Tema.VerdeEscuro; editor.Controls.Add(estado);
            if (nome == "ClientesForm" || nome == "UsuariosForm")
            {
                Secao(editor, "Identificação", m, "camponome|lblnome", "campocpf|lblcpf", "camporg|lblrg", "campodata_nascimento|lbldata_nascimento");
                Secao(editor, "Contato", m, "campoemail|lblemail", "campotelefone|lbltelefone", "campocidade|lblcidade");
                if (nome == "ClientesForm") Secao(editor, "Endereço e situação", m, "campoendereco|lblendereco", "campoativo|lblativo");
                else Secao(editor, "Acesso ao sistema", m, "campoperfil|lblperfil", "campoativo|lblativo", "camposenha|lblsenha");
            }
            else if (nome == "AparelhosForm")
            {
                Secao(editor, "Cliente e identificação", m, "campocliente_id|lblcliente_id", "campotipo|lbltipo", "campomarca|lblmarca", "campomodelo|lblmodelo", "camponumero_serie|lblnumero_serie", "campocor|lblcor");
                Secao(editor, "Condições de recebimento", m, "campoacessorios|lblacessorios", "campoestado_fisico|lblestado_fisico", "campoobservacoes|lblobservacoes");
            }
            else
            {
                Secao(editor, "Identificação da peça", m, "campocodigo|lblcodigo", "camponome|lblnome", "campoativo|lblativo");
                Secao(editor, "Compatibilidade", m, "campotipo|lbltipo", "campomarca|lblmarca", "campomodelo_compativel|lblmodelo_compativel");
                Secao(editor, "Valores e estoque", m, "campoestoque_minimo|lblestoque_minimo", "campovalor_compra|lblvalor_compra", "campovalor_venda|lblvalor_venda");
            }
            Button voltar = new Button { Text = "Voltar à consulta" }; Tema.EstilizarBotao(voltar);
            voltar.Click += delegate { abas.SelectedIndex = 0; };
            editor.Controls.Add(Barra(m["btnSalvar"], voltar));
            m["btnNovo"].Click += delegate { abas.SelectedIndex = 1; };
            m["btnEditar"].Click += delegate { if (m["lblEdicao"].Text.StartsWith("Editando")) abas.SelectedIndex = 1; };
        }

        private static void MontarConsulta(TableLayoutPanel estrutura, Dictionary<string, Control> m, string nome)
        {
            TabControl abas = Abas(estrutura, nome == "OrdensForm" ? "Ordens de serviço" : "Histórico");
            TableLayoutPanel lista = Lista(abas.TabPages[0]);
            lista.Controls.Add(Barra(Escolher(m, "cmbStatus", "txtBusca", "btnPesquisar", "btnNova")), 0, 0);
            if (m.ContainsKey("txtBusca")) m["txtBusca"].AccessibleName = "Pesquisar ordem, cliente ou aparelho";
            m["grade"].Dock = DockStyle.Fill; lista.Controls.Add(m["grade"], 0, 1);
            lista.Controls.Add(Barra(Escolher(m, "btnDetalhes", "btnExportar", "lblDica")), 0, 2);
        }

        private static void MontarFormulario(TableLayoutPanel estrutura, Dictionary<string, Control> m, string nome)
        {
            TabControl abas = Abas(estrutura, nome == "PerfilForm" ? "Conta / Perfil" : nome == "MovimentoForm" ? "Movimentação" : "Recebimento");
            FlowLayoutPanel editor = Coluna(abas.TabPages[0]);
            if (nome == "PerfilForm")
            {
                Secao(editor, "Meus dados", m, "txtNome|lblNome", "txtEmail|lblEmail", "txtTelefone|lblTelefone");
                Secao(editor, "Segurança", m, "txtAtual|lblAtual", "txtNova|lblNova", "txtConfirmacao|lblConfirmacao");
            }
            else if (nome == "AbrirOrdemForm")
            {
                Secao(editor, "Cliente e aparelho", m, "cmbCliente|lblCliente", "cmbAparelho|lblAparelho");
                Secao(editor, "Solicitação de manutenção", m, "txtProblema|lblProblema", "dtPrevisao|lblPrevisao", "txtObservacoes|lblObservacoes");
            }
            else
            {
                m["lblPeca"].Height = 36; editor.Controls.Add(m["lblPeca"]);
                Secao(editor, "Movimentar estoque", m, "cmbTipo|lblTipo", "numQuantidade|lblQuantidade", "txtMotivo|lblMotivo");
                editor.Controls.Add(m["lblDica"]);
            }
            editor.Controls.Add(Barra(m["btnSalvar"]));
        }

        private static void MontarOrdem(TableLayoutPanel estrutura, Dictionary<string, Control> m)
        {
            TabControl abas = Abas(estrutura, "Recebimento", "Diagnóstico e serviço", "Peças utilizadas", "Conclusão e histórico");
            FlowLayoutPanel recebimento = Coluna(abas.TabPages[0]);
            m["txtRecebimento"].Height = 340;
            recebimento.Controls.Add(m["txtRecebimento"]);
            recebimento.Controls.Add(m["lblDica"]);
            FlowLayoutPanel diagnostico = Coluna(abas.TabPages[1]);
            Secao(diagnostico, "Manutenção", m, "txtDiagnostico|lblDiagnostico", "txtServico|lblServico");
            Secao(diagnostico, "Andamento e valores", m, "cmbStatus|lblStatus", "numMaoObra|lblMaoObra", "numDesconto|lblDesconto");
            diagnostico.Controls.Add(Barra(m["btnSalvar"], m["btnAtualizar"]));
            TableLayoutPanel pecas = Lista(abas.TabPages[2]);
            pecas.Controls.Add(Barra(m["lblPecas"], m["cmbPeca"], m["numQuantidade"], m["btnAdicionar"]), 0, 0);
            m["cmbPeca"].Width = 360; m["numQuantidade"].Width = 80;
            m["numQuantidade"].AccessibleName = "Quantidade de peças";
            m["grade"].Dock = DockStyle.Fill; pecas.Controls.Add(m["grade"], 0, 1);
            pecas.Controls.Add(Barra(m["btnDevolver"], m["btnExportar"]), 0, 2);
            FlowLayoutPanel conclusao = Coluna(abas.TabPages[3]);
            m["lblTotal"].Font = new Font("Segoe UI", 20, FontStyle.Bold); m["lblTotal"].Height = 60;
            conclusao.Controls.Add(m["lblTotal"]);
            Label orientacao = new Label { Text = "Conclua o serviço na aba Diagnóstico e serviço. Após a conclusão, o atendente pode registrar a entrega ao cliente.", Height = 70, ForeColor = Tema.Texto };
            conclusao.Controls.Add(orientacao);
            conclusao.Controls.Add(Barra(m["btnEntregar"], m["btnHistorico"]));
        }
    }
}
