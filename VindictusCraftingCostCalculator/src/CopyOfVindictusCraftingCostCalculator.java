import java.awt.Color;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.Label;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URI;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextArea;
import javax.swing.JTextField;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

/**
 * 
 * @author Aurion / aurion@gmail.com
 * 
 */
public class CopyOfVindictusCraftingCostCalculator extends JFrame implements
ActionListener {

	private static final long serialVersionUID = 1L;
	static JFrame frame;
	static JTextArea display = new JTextArea();
	static String text;
	static JTextField entryField = new JTextField();
	static JPanel panel;
	public static JTextArea messages = new JTextArea();
	static CopyOfVindictusCraftingCostCalculator vinccc;
	FlowLayout layout = new FlowLayout(FlowLayout.LEFT);
	static int labelLength = 0;
	static boolean craftedByPlayer = false;
	static String playerCrafted = new String("Player");
	static String npcCrafted = new String("NPC");
	JRadioButton radioButtonPlayerCrafted = new JRadioButton(CopyOfVindictusCraftingCostCalculator.playerCrafted);
	JRadioButton radioButtonNpcCrafted = new JRadioButton(CopyOfVindictusCraftingCostCalculator.npcCrafted);
	static Double[] results;
	static Double fee;
	static Double cost;
	static Set<?> set;

	public static void main(String[] args) {
		CopyOfVindictusCraftingCostCalculator.vinccc = new CopyOfVindictusCraftingCostCalculator();
	}

	public CopyOfVindictusCraftingCostCalculator() {
		CopyOfVindictusCraftingCostCalculator.panel = new JPanel();

		this.setAlwaysOnTop(true);
		JButton submit = new JButton("Submit");
		CopyOfVindictusCraftingCostCalculator.messages.setEditable(false);
		CopyOfVindictusCraftingCostCalculator.messages.setPreferredSize(new Dimension(200, 0));
		this.setMinimumSize(new Dimension(350, 75));

		CopyOfVindictusCraftingCostCalculator.panel.add(CopyOfVindictusCraftingCostCalculator.messages);
		CopyOfVindictusCraftingCostCalculator.entryField.setPreferredSize(new Dimension(200, 25));
		CopyOfVindictusCraftingCostCalculator.entryField.addKeyListener(new KeyListener() {

			@Override
			public void keyTyped(KeyEvent e) {
			}

			@Override
			public void keyReleased(KeyEvent e) {
			}

			@Override
			public void keyPressed(KeyEvent e) {
				if (e.getKeyCode() == 10) {
					CopyOfVindictusCraftingCostCalculator.text = CopyOfVindictusCraftingCostCalculator.entryField.getText();
					CopyOfVindictusCraftingCostCalculator.messages.setText("");
					try {
						CopyOfVindictusCraftingCostCalculator.readItemFromWiki(CopyOfVindictusCraftingCostCalculator.text);
					} catch (ClientProtocolException e1) {
						e1.printStackTrace();
					} catch (IOException e1) {
						e1.printStackTrace();
					}
				}
			}
		});
		CopyOfVindictusCraftingCostCalculator.panel.setLayout(this.layout);
		CopyOfVindictusCraftingCostCalculator.panel.add(new Label("Enter the name of the item or set"));
		CopyOfVindictusCraftingCostCalculator.panel.add(CopyOfVindictusCraftingCostCalculator.entryField);

		submit.setPreferredSize(new Dimension(100, 25));
		submit.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				CopyOfVindictusCraftingCostCalculator.text = CopyOfVindictusCraftingCostCalculator.entryField.getText();
				CopyOfVindictusCraftingCostCalculator.messages.setText("");
				try {
					CopyOfVindictusCraftingCostCalculator.readItemFromWiki(CopyOfVindictusCraftingCostCalculator.text);
				} catch (ClientProtocolException e1) {
					e1.printStackTrace();
				} catch (IOException e1) {
					e1.printStackTrace();
				}
			}
		});

		CopyOfVindictusCraftingCostCalculator.panel.add(submit);

		ButtonGroup radioButtonGroup = new ButtonGroup();

		ActionListener actionListener = new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				if (e.getActionCommand() == CopyOfVindictusCraftingCostCalculator.playerCrafted) {
					CopyOfVindictusCraftingCostCalculator.craftedByPlayer = true;
					CopyOfVindictusCraftingCostCalculator.setOutput(CopyOfVindictusCraftingCostCalculator.fee, CopyOfVindictusCraftingCostCalculator.cost, CopyOfVindictusCraftingCostCalculator.results);
				} else {
					CopyOfVindictusCraftingCostCalculator.craftedByPlayer = false;
					CopyOfVindictusCraftingCostCalculator.setOutput(CopyOfVindictusCraftingCostCalculator.fee, CopyOfVindictusCraftingCostCalculator.cost, CopyOfVindictusCraftingCostCalculator.results);
				}
			}
		};
		this.radioButtonPlayerCrafted.setActionCommand(CopyOfVindictusCraftingCostCalculator.playerCrafted);
		this.radioButtonPlayerCrafted.addActionListener(actionListener);

		this.radioButtonNpcCrafted.setActionCommand(CopyOfVindictusCraftingCostCalculator.npcCrafted);
		this.radioButtonNpcCrafted.addActionListener(actionListener);
		this.radioButtonNpcCrafted.setSelected(true);
		radioButtonGroup.add(this.radioButtonPlayerCrafted);
		radioButtonGroup.add(this.radioButtonNpcCrafted);

		CopyOfVindictusCraftingCostCalculator.panel.add(new JLabel("Crafted by"));
		CopyOfVindictusCraftingCostCalculator.panel.add(this.radioButtonPlayerCrafted);
		CopyOfVindictusCraftingCostCalculator.panel.add(this.radioButtonNpcCrafted);

		this.add(CopyOfVindictusCraftingCostCalculator.panel);

		this.setTitle("Vindictus Crafting Cost Calculator");
		this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setSize(240, 140);
		this.setVisible(true);
		CopyOfVindictusCraftingCostCalculator.entryField.requestFocusInWindow();
	}

	public static void readItemFromWiki(String itemName)
			throws ClientProtocolException, IOException {
		HttpClient httpclient = new DefaultHttpClient();
		itemName = itemName.replaceAll(",", " ");
		String[] itemNameSplitted = new String[itemName.split(" ").length + 1];
		itemNameSplitted = itemName.split(" ");

		itemName = "";
		try {
			for (int i = 0; i < itemNameSplitted.length; i++) {
				itemNameSplitted[i] = itemNameSplitted[i].substring(0, 1)
						.toUpperCase() + itemNameSplitted[i].substring(1);
				itemName += itemNameSplitted[i] + "_";
			}
		} catch (StringIndexOutOfBoundsException e) {
			CopyOfVindictusCraftingCostCalculator.messages.setText("You need to enter something first");
		}
		itemName = itemName.substring(0, itemName.length() - 1);
		CopyOfVindictusCraftingCostCalculator.text = itemName;
		CopyOfVindictusCraftingCostCalculator.messages.setText("Searching for: " + itemName);
		HashMap<String, Double> materials = new HashMap<String, Double>();
		boolean foundSomething = false;
		try {
			String uriString = "http://www.vindictuswiki.com/w/index.php?title="
					+ itemName + "&action=edit";
			HttpGet httpget = new HttpGet(new URI(uriString));
			HttpResponse response = httpclient.execute(httpget);
			HttpEntity entity = response.getEntity();
			BufferedReader reader = new BufferedReader(new InputStreamReader(
					entity.getContent()));
			String line;
			while ((line = reader.readLine()) != null) {
				if (!CopyOfVindictusCraftingCostCalculator.craftedByPlayer) {
					if (line.contains("{{Obtain Fee|")) {
						foundSomething = true;
						CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + "\n" + line);
						line = line.replace(",", "");
						Double goldCost = Double.parseDouble(line.substring(
								line.lastIndexOf("|") + 5, line.length() - 2));
						materials.put("Gold", goldCost);
					}
					if (line.contains("Expertise")) {
						break;
					}
				} else if (CopyOfVindictusCraftingCostCalculator.craftedByPlayer) {
					if (line.contains("Expertise")) {
						foundSomething = true;
						materials.put("Gold", 0d);
					}
				}
				CopyOfVindictusCraftingCostCalculator.parseMaterials(materials, line);
			}

		} catch (Exception e) {
			e.printStackTrace();
		}
		if (foundSomething) {

			CopyOfVindictusCraftingCostCalculator.fee = materials.get("Gold");
			materials.remove("Gold");
			CopyOfVindictusCraftingCostCalculator.set = materials.entrySet();

			CopyOfVindictusCraftingCostCalculator.cost = CopyOfVindictusCraftingCostCalculator.fee;

			String[] labels = new String[materials.keySet().size()];
			labels = materials.keySet().toArray(labels);
			CopyOfVindictusCraftingCostCalculator.labelLength = labels.length;
			int var = 0;
			Iterator<?> it = materials.values().iterator();
			while (it.hasNext()) {
				labels[var] = labels[var] + " (x"
						+ (int) Double.parseDouble(it.next().toString()) + ")";
				var++;
			}

			JOptionPaneMultiInput multiInput = new JOptionPaneMultiInput(
					labels, CopyOfVindictusCraftingCostCalculator.messages);
			multiInput.toFront();
			CopyOfVindictusCraftingCostCalculator.display = new JTextArea();
			int row = 20;

			CopyOfVindictusCraftingCostCalculator.messages.setPreferredSize(new Dimension(300,
					((row * labels.length) + 100)));
			CopyOfVindictusCraftingCostCalculator.vinccc.setMinimumSize(new Dimension(375,
					((row * labels.length) + 250)));
			CopyOfVindictusCraftingCostCalculator.results = JOptionPaneMultiInput.results;


			CopyOfVindictusCraftingCostCalculator.setOutput(CopyOfVindictusCraftingCostCalculator.fee, CopyOfVindictusCraftingCostCalculator.cost, CopyOfVindictusCraftingCostCalculator.results);
		}

		else {
			CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + "\n"
					+ "No item matching the search found");
			CopyOfVindictusCraftingCostCalculator.messages.setForeground(Color.red);
		}

	}

	public static void setOutput(Double fee, Double cost,
			Double[] results) {
		int j = 0;
		CopyOfVindictusCraftingCostCalculator.messages.setText("You need the following: " + "\n\n");
		if (!CopyOfVindictusCraftingCostCalculator.craftedByPlayer) {
			CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + "Crafting fee: " + fee + "\n");
		}
		Double[] costPerItem = new Double[CopyOfVindictusCraftingCostCalculator.set.size()];
		Iterator<?> i = CopyOfVindictusCraftingCostCalculator.set.iterator();
		while (i.hasNext()) {
			costPerItem[j] = results[j];
			@SuppressWarnings("rawtypes")
			Map.Entry me = (Map.Entry) i.next();

			costPerItem[j] = (Double) me.getValue() * costPerItem[j];
			cost += costPerItem[j];
			CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + me.getKey() + " x"
					+ (int) Double.parseDouble(me.getValue().toString())
					+ " : " + costPerItem[j].intValue() + " Gold" + "\n");
			j++;
		}
		CopyOfVindictusCraftingCostCalculator.text = CopyOfVindictusCraftingCostCalculator.text.replace("_", " ");
		CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + "\n" + CopyOfVindictusCraftingCostCalculator.text + ": "
				+ cost.intValue() + " Gold");
		CopyOfVindictusCraftingCostCalculator.messages.setAlignmentX(Component.CENTER_ALIGNMENT);
	}

	public static void parseMaterials(HashMap<String, Double> materials,
			String line) {
		if (line.contains("{{Materials|")) {
			CopyOfVindictusCraftingCostCalculator.messages.setText(CopyOfVindictusCraftingCostCalculator.messages.getText() + "\n" + line);

			String materialName = line.substring(line.indexOf("|") + 1);

			materialName = materialName.substring(0, materialName.indexOf("|"));
			String materialCountAsString = line.substring(13 + materialName
					.length());
			int nextSeperator = materialCountAsString.indexOf("|");
			if (nextSeperator != -1) {
				materialCountAsString = materialCountAsString.substring(0,
						nextSeperator);
			} else {
				materialCountAsString = materialCountAsString.substring(0,
						materialCountAsString.length() - 2);
			}
			Double materialCount = Double.parseDouble(materialCountAsString);
			materials.put(materialName, materialCount);
		}
	}

	@Override
	public void actionPerformed(ActionEvent e) {
		if (e.getSource() == this.radioButtonPlayerCrafted) {
			CopyOfVindictusCraftingCostCalculator.craftedByPlayer = true;
		} else if (e.getSource() == this.radioButtonNpcCrafted) {
			CopyOfVindictusCraftingCostCalculator.craftedByPlayer = false;
		}

	}

}
